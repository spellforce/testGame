using Godot;

namespace GameLogic.Game
{
    /// <summary>3D 主界面：把纯 2D 操作区作为纹理贴到平面上。</summary>
    [Tool]
    public partial class MainInterface : Node3D
    {
        private const string OperationAreaScenePath = "res://TheGame/Scenes/GameOperationArea.tscn";
        private const float PlaneWidth = 16f;
        private const float PlaneHeight = 9f;
        private const float CameraDistance = 10f;
        private const float PlaneShearDegrees = 5.7f;

        private SubViewport m_OperationViewport;
        private MeshInstance3D m_OperationPlane;
        private Camera3D m_PresentationCamera;

        public override void _Ready()
        {
            Name = "MainInterface";
            BuildWorldViewport();
            BuildPresentationCamera();
            if (!Engine.IsEditorHint())
            {
                var gameBoard = m_OperationViewport.GetNodeOrNull<GameBoard>("GameOperationArea");
                var hud = GetNodeOrNull<Hud.GameHud>("GameHud");
                gameBoard?.BindHud(hud);
            }
            GetViewport().SizeChanged += SyncViewportSize;
        }

        public override void _Input(InputEvent @event)
        {
            if (!Engine.IsEditorHint() && m_OperationViewport != null)
            {
                m_OperationViewport.PushInput(@event);
            }
        }

        private void BuildWorldViewport()
        {
            var size = GetViewport().GetVisibleRect().Size;
            m_OperationViewport = GetNodeOrNull<SubViewport>("OperationViewport") ?? new SubViewport
            {
                Name = "OperationViewport",
                Size = ToViewportSize(size),
                RenderTargetUpdateMode = SubViewport.UpdateMode.Always,
                TransparentBg = false,
                CanvasItemDefaultTextureFilter = Viewport.DefaultCanvasItemTextureFilter.Nearest,
            };
            if (m_OperationViewport.GetParent() == null) AddChild(m_OperationViewport);

            if (m_OperationViewport.GetChildCount() == 0)
            {
                var operationArea = GD.Load<PackedScene>(OperationAreaScenePath)?.Instantiate<Node2D>();
                if (operationArea == null)
                {
                    GD.PushError($"Cannot load operation area scene: {OperationAreaScenePath}");
                    return;
                }
                m_OperationViewport.AddChild(operationArea);
            }
        }

        private void BuildPresentationCamera()
        {
            var stage = GetNodeOrNull<Node3D>("PresentationStage") ?? new Node3D { Name = "PresentationStage" };
            if (stage.GetParent() == null) AddChild(stage);

            m_OperationPlane = stage.GetNodeOrNull<MeshInstance3D>("OperationPlane") ?? new MeshInstance3D
            {
                Name = "OperationPlane",
                Mesh = new QuadMesh { Size = new Vector2(PlaneWidth, PlaneHeight) },
            };
            if (m_OperationPlane.GetParent() == null) stage.AddChild(m_OperationPlane);
            m_OperationPlane.Mesh ??= new QuadMesh { Size = new Vector2(PlaneWidth, PlaneHeight) };
            m_OperationPlane.MaterialOverride = new ShaderMaterial
            {
                Shader = new Shader
                {
                    Code = $$"""
                        shader_type spatial;
                        render_mode unshaded, cull_disabled;

                        uniform sampler2D operation_texture : source_color, filter_nearest;
                        const float SHEAR_SLOPE = {{Mathf.Tan(Mathf.DegToRad(PlaneShearDegrees)).ToString(System.Globalization.CultureInfo.InvariantCulture)}};

                        void vertex() {
                            VERTEX.x += SHEAR_SLOPE * VERTEX.y;
                        }

                        void fragment() {
                            ALBEDO = texture(operation_texture, UV).rgb;
                        }
                        """
                },
            };
            ((ShaderMaterial)m_OperationPlane.MaterialOverride).SetShaderParameter("operation_texture", m_OperationViewport.GetTexture());
            m_PresentationCamera = stage.GetNodeOrNull<Camera3D>("PresentationCamera") ?? new Camera3D
            {
                Name = "PresentationCamera",
                Projection = Camera3D.ProjectionType.Orthogonal,
                Size = PlaneHeight,
                Position = new Vector3(0f, 0f, CameraDistance),
                RotationDegrees = Vector3.Zero,
                Current = true,
            };
            if (m_PresentationCamera.GetParent() == null) stage.AddChild(m_PresentationCamera);
            m_PresentationCamera.Projection = Camera3D.ProjectionType.Orthogonal;
            m_PresentationCamera.Size = PlaneHeight;
            m_PresentationCamera.Position = new Vector3(0f, 0f, CameraDistance);
            m_PresentationCamera.RotationDegrees = Vector3.Zero;
            m_PresentationCamera.Current = true;
        }

        private void SyncViewportSize()
        {
            if (GodotObject.IsInstanceValid(m_OperationViewport))
            {
                m_OperationViewport.Size = ToViewportSize(GetViewport().GetVisibleRect().Size);
            }
        }

        private static Vector2I ToViewportSize(Vector2 size)
            => new(Mathf.Max(1, (int)size.X), Mathf.Max(1, (int)size.Y));
    }
}
