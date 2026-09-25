using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameCamera : MonoBehaviour
{
	public static GameCamera instance;

	public float MoveSpeed = 1f;

	public float ZoomSpeed = 1f;

	public float MinZoom = 3f;

	public float MaxZoom = 11f;

	public Vector3 cameraStartPosition;

	public Vector3 cameraTargetPosition;

	public float CameraMoveSpeed = 12f;

	private Vector2 startInputPosition;

	private Vector3 startPosition;

	private float startTime;

	private bool isTouchDraggingCard;

	private Vector3 touchCameraStartPosition;

	private Vector3 zoomStartPosition;

	private float startZoom;

	private float startDist;

	private float prevInputCount;

	private bool isDraggingCamera;

	private Vector3? _targetPositionOverride;

	private IGameCardOrCardData _targetCardOverride;

	public float? CameraPositionDistanceOverride;

	public PostProcessVolume PauseVolume;

	public PostProcessVolume FocusVolume;

	public PostProcessVolume SpiritVolume;

	public PostProcessVolume EnergyVolume;

	public ImageEffect SpiritImageEffect;

	public ImageEffect SpiritTransitionEffect;

	public Material SpiritBackgroundMaterial;

	[HideInInspector]
	public Material TempSpiritBackgroundMaterial;

	public Vector3 GameStartCameraPosition;

	public Camera MyCam;

	public float Screenshake;

	private Draggable currentlySelectedDraggable;

	private float spiritEffectStrength;

	public bool IsDragging => isDraggingCamera;

	public Vector3? TargetPositionOverride
	{
		get
		{
			return _targetPositionOverride;
		}
		set
		{
			_targetCardOverride = null;
			_targetPositionOverride = value;
		}
	}

	public IGameCardOrCardData TargetCardOverride
	{
		get
		{
			if (_targetCardOverride is CardData cardData && (Object)(object)cardData.MyGameCard == (Object)null)
			{
				_targetCardOverride = null;
				return null;
			}
			return _targetCardOverride;
		}
		set
		{
			_targetPositionOverride = null;
			_targetCardOverride = value;
		}
	}

	private bool canControlCamera
	{
		get
		{
			if (WorldManager.instance.IsPlaying && !TargetPositionOverride.HasValue && (Object)(object)WorldManager.instance.IntroPack == (Object)null)
			{
				return !GameScreen.instance.ControllerIsInUI;
			}
			return false;
		}
	}

	public void CenterOnBoard(GameBoard board)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = (((Component)this).transform.position = board.MiddleOfBoard() + GameStartCameraPosition);
		cameraTargetPosition = val;
	}

	public void KeepCameraAtCurrentPos()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		cameraTargetPosition = ((Component)this).transform.position;
	}

	private void Awake()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		instance = this;
		TempSpiritBackgroundMaterial = new Material(SpiritBackgroundMaterial);
	}

	private Transform GetIntroCameraTransform()
	{
		GameBoard gameBoard = (WorldManager.instance.IsCitiesDlcActive() ? WorldManager.instance.GetBoardWithId("cities") : ((!WorldManager.instance.IsSpiritDlcActive()) ? WorldManager.instance.GetBoardWithId("main") : WorldManager.instance.GetBoardWithId("death")));
		return gameBoard.CameraIntroPosition;
	}

	private void Start()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = GetIntroCameraTransform().position;
		cameraStartPosition = ((Component)this).transform.position;
		cameraTargetPosition = ((Component)this).transform.position;
	}

	public Vector3 ScreenPosToWorldPos(Vector3 p)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Ray val = MyCam.ScreenPointToRay(p);
		Plane val2 = default(Plane);
		((Plane)(ref val2))._002Ector(Vector3.up, Vector3.zero);
		float num = default(float);
		((Plane)(ref val2)).Raycast(val, ref num);
		return ((Ray)(ref val)).origin + ((Ray)(ref val)).direction * num;
	}

	public Vector3 ScreenPosToWorldPos(Vector2 pos, Vector3 camPos)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)MyCam).transform.position;
		((Component)MyCam).transform.position = camPos;
		Vector3 result = ScreenPosToWorldPos(Vector2.op_Implicit(pos));
		((Component)MyCam).transform.position = position;
		return result;
	}

	public void StartDragging()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (canControlCamera && InputController.instance.InputCount != 2)
		{
			startInputPosition = InputController.instance.GetInputPosition(0);
			startPosition = ((Component)this).transform.position;
			startTime = Time.time;
			isDraggingCamera = true;
		}
	}

	private Draggable FindNextDraggableInDirection(Vector3 curPos, Vector3 wantedDir)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MinValue;
		Draggable result = null;
		foreach (Draggable allDraggable in WorldManager.instance.AllDraggables)
		{
			if (!((Component)allDraggable).gameObject.activeInHierarchy || (Object)(object)allDraggable == (Object)(object)currentlySelectedDraggable || (Object)(object)allDraggable == (Object)(object)WorldManager.instance.DraggingDraggable)
			{
				continue;
			}
			if ((Object)(object)allDraggable.MyBoard == (Object)null)
			{
				Debug.Log((object)(((object)allDraggable)?.ToString() + " does not have a board"));
			}
			else
			{
				if (!allDraggable.MyBoard.IsCurrent || !allDraggable.CanBeAutoMovedTo)
				{
					continue;
				}
				Vector3 val = allDraggable.AutoMoveSnapPosition - curPos;
				float num2 = Vector3.Dot(wantedDir, val);
				if (!(num2 <= 0f))
				{
					float num3 = num2 / ((Vector3)(ref val)).sqrMagnitude;
					if (num3 > num)
					{
						num = num3;
						result = allDraggable;
					}
				}
			}
		}
		return result;
	}

	private Draggable FindNextDraggable(Vector2 snapMoveInput)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		return FindNextDraggableInDirection(WorldManager.instance.mouseWorldPosition, new Vector3(snapMoveInput.x, 0f, snapMoveInput.y));
	}

	private void Update()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_074c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_077b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0824: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Unknown result type (might be due to invalid IL or missing references)
		//IL_084d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0852: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08da: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0911: Unknown result type (might be due to invalid IL or missing references)
		//IL_0916: Unknown result type (might be due to invalid IL or missing references)
		//IL_0923: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_0889: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Unknown result type (might be due to invalid IL or missing references)
		//IL_089a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
		Screenshake -= Time.deltaTime;
		Vector3 val;
		if (Screenshake > 0f && AccessibilityScreen.ScreenshakeEnabled)
		{
			Vector2 insideUnitCircle = Random.insideUnitCircle;
			val = Screenshake * (((Component)this).transform.right * insideUnitCircle.x + ((Component)this).transform.up * insideUnitCircle.y);
		}
		else
		{
			val = Vector3.zero;
		}
		if (isDraggingCamera && !canControlCamera)
		{
			isDraggingCamera = false;
		}
		if (isDraggingCamera && InputController.instance.InputCount == 2)
		{
			isDraggingCamera = false;
		}
		Vector2 move = InputController.instance.GetMove();
		Vector3 zero = default(Vector3);
		((Vector3)(ref zero))._002Ector(move.x, 0f, move.y);
		Vector2 snapMoveInput = InputController.instance.GetSnapMovePressed();
		if (!canControlCamera)
		{
			zero = Vector3.zero;
			snapMoveInput = Vector2.zero;
		}
		if (((Vector2)(ref snapMoveInput)).magnitude > 0f)
		{
			if ((Object)(object)WorldManager.instance.DraggingCard != (Object)null)
			{
				WorldManager.instance.grabOffset = WorldManager.instance.DraggingCard.CardNameText.transform.position - ((Component)WorldManager.instance.DraggingCard).transform.position;
			}
			Draggable draggable = FindNextDraggable(snapMoveInput);
			if ((Object)(object)draggable != (Object)null)
			{
				currentlySelectedDraggable = draggable;
				cameraTargetPosition = draggable.AutoMoveSnapPosition - GetCurrentGroundOffset();
			}
		}
		if (((Vector3)(ref zero)).magnitude > 0.01f)
		{
			currentlySelectedDraggable = null;
		}
		cameraTargetPosition += zero * Time.deltaTime * (MoveSpeed + Mathf.Clamp(cameraTargetPosition.y / 2f - 4f, 0f, 10f));
		bool flag = canControlCamera;
		if (InputController.instance.CurrentSchemeIsMouseKeyboard && GameCanvas.instance.MousePositionIsOverUI())
		{
			flag = false;
		}
		if (WorldManager.instance.InAnimation && !WorldManager.instance.CutsceneBoardView)
		{
			flag = false;
		}
		if (InputController.instance.InputCount == 2)
		{
			Vector2 inputPosition = InputController.instance.GetInputPosition(0);
			Vector2 inputPosition2 = InputController.instance.GetInputPosition(1);
			float num = Vector2.Distance(inputPosition, inputPosition2);
			if (prevInputCount != 2f && flag)
			{
				Debug.Log((object)"Started zooming");
				touchCameraStartPosition = ((Component)this).transform.position;
				zoomStartPosition = Vector2.op_Implicit(Vector2.Lerp(inputPosition, inputPosition2, 0.5f));
				startZoom = ((Component)this).transform.position.y;
				startDist = num;
			}
			if (flag && num > float.Epsilon)
			{
				Vector2 val2 = Vector2.Lerp(inputPosition, inputPosition2, 0.5f);
				Vector3 val3 = ScreenPosToWorldPos(Vector2.op_Implicit(val2)) - ScreenPosToWorldPos(zoomStartPosition);
				cameraTargetPosition = touchCameraStartPosition - val3;
				Vector3 val4 = ScreenPosToWorldPos(val2, cameraTargetPosition);
				float num2 = MaxZoom + WorldManager.instance.CurrentBoard.WorldSizeIncrease * 2.7f;
				cameraTargetPosition.y = Mathf.Clamp(startZoom * (startDist / num), MinZoom, num2);
				Vector3 val5 = ScreenPosToWorldPos(val2, cameraTargetPosition);
				Vector3 val6 = val4 - val5;
				val6.y = 0f;
				cameraTargetPosition += val6;
				((Component)this).transform.position = cameraTargetPosition;
			}
		}
		if (flag && !InputController.instance.CurrentSchemeIsTouch)
		{
			float num3 = InputController.instance.GetZoom() * 0.2f;
			Vector2 pos = InputController.instance.ClampedMousePosition();
			if (InputController.instance.CurrentSchemeIsController)
			{
				pos = new Vector2((float)Screen.width, (float)Screen.height) * 0.5f;
			}
			Vector3 val7 = ScreenPosToWorldPos(pos, cameraTargetPosition);
			val7.y = cameraTargetPosition.y;
			float y = cameraTargetPosition.y;
			cameraTargetPosition.y += ZoomSpeed * num3;
			float num4 = MaxZoom + WorldManager.instance.CurrentBoard.WorldSizeIncrease * 2.7f;
			cameraTargetPosition.y = Mathf.Clamp(cameraTargetPosition.y, MinZoom, num4);
			if (y != cameraTargetPosition.y && Mathf.Abs(num3) > 0.0001f)
			{
				Vector3 val8 = ScreenPosToWorldPos(pos, cameraTargetPosition);
				Vector3 val9 = val7 - val8;
				val9.y = 0f;
				cameraTargetPosition += val9;
			}
		}
		if (Object.op_Implicit((Object)(object)WorldManager.instance.DraggingCard))
		{
			if (InputController.instance.CurrentSchemeIsTouch)
			{
				isTouchDraggingCard = true;
			}
		}
		else
		{
			isTouchDraggingCard = false;
		}
		if (isTouchDraggingCard)
		{
			float num5 = 0.5f * cameraTargetPosition.y;
			if (InputController.instance.GetInputPosition(0).y >= (float)Screen.height * 0.8f)
			{
				float num6 = Mathf.InverseLerp((float)Screen.height * 0.8f, (float)Screen.height, InputController.instance.GetInputPosition(0).y) * num5;
				cameraTargetPosition += Vector3.forward * Time.deltaTime * num6;
			}
			else if (InputController.instance.GetInputPosition(0).y <= (float)Screen.height * 0.2f)
			{
				float num7 = Mathf.InverseLerp((float)Screen.height * 0.2f, 0f, InputController.instance.GetInputPosition(0).y) * num5;
				cameraTargetPosition += Vector3.back * Time.deltaTime * num7;
			}
			if (InputController.instance.GetInputPosition(0).x >= (float)Screen.width * 0.8f)
			{
				float num8 = Mathf.InverseLerp((float)Screen.width * 0.8f, (float)Screen.width, InputController.instance.GetInputPosition(0).x) * num5;
				cameraTargetPosition += Vector3.right * Time.deltaTime * num8;
			}
			else if (InputController.instance.GetInputPosition(0).x <= (float)Screen.width * 0.2f)
			{
				float num9 = Mathf.InverseLerp((float)Screen.width * 0.2f, 0f, InputController.instance.GetInputPosition(0).x) * num5;
				cameraTargetPosition += Vector3.left * Time.deltaTime * num9;
			}
		}
		if (isDraggingCamera && InputController.instance.GetInput(0))
		{
			Vector3 val10 = ScreenPosToWorldPos(Vector2.op_Implicit(InputController.instance.GetInputPosition(0))) - ScreenPosToWorldPos(Vector2.op_Implicit(startInputPosition));
			Vector3 val11 = (((Component)this).transform.position = startPosition - val10);
			cameraTargetPosition = val11;
		}
		if (isDraggingCamera && InputController.instance.GetInputEnded(0))
		{
			Vector2 val13 = startInputPosition - InputController.instance.GetInputPosition(0);
			if (Time.time - startTime < 0.2f && ((Vector2)(ref val13)).magnitude <= 5f)
			{
				Clicked();
			}
			isDraggingCamera = false;
		}
		Vector3 val14 = cameraTargetPosition;
		bool flag2 = true;
		bool flag3 = TargetPositionOverride.HasValue || TargetCardOverride != null;
		Vector3? val15 = TargetPositionOverride;
		if (TargetCardOverride != null)
		{
			val15 = TargetCardOverride.Position;
		}
		if ((Object)(object)WorldManager.instance.IntroPack != (Object)null)
		{
			val15 = ((Component)WorldManager.instance.IntroPack).transform.position;
			cameraTargetPosition = ((Component)this).transform.position;
			flag3 = true;
		}
		if (val15.HasValue)
		{
			Vector3 value = val15.Value;
			value.y = 0.01f;
			float num10 = (CameraPositionDistanceOverride.HasValue ? CameraPositionDistanceOverride.Value : 7f);
			val14 = value - ((Component)this).transform.forward * num10;
			flag2 = false;
		}
		if (WorldManager.instance.CurrentGameState == WorldManager.GameState.InMenu)
		{
			val14 = (cameraTargetPosition = GetIntroCameraTransform().position);
			flag2 = false;
		}
		((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, val14 + val * 0.5f, Time.deltaTime * CameraMoveSpeed);
		if (flag2)
		{
			((Component)this).transform.position = ClampPos(((Component)this).transform.position);
			cameraTargetPosition = ClampPos(cameraTargetPosition);
		}
		((Behaviour)PauseVolume).enabled = WorldManager.instance.SpeedUp == 0f && !WorldManager.instance.InAnimation;
		((Component)PauseVolume).gameObject.SetActive(WorldManager.instance.SpeedUp == 0f && !WorldManager.instance.InAnimation);
		if (WorldManager.instance.currentAnimation != null || WorldManager.instance.currentAnimationRoutine != null)
		{
			flag3 = true;
		}
		if (WorldManager.instance.CurrentGameState != WorldManager.GameState.Playing)
		{
			flag3 = true;
		}
		FocusVolume.weight = Mathf.Lerp(FocusVolume.weight, flag3 ? 1f : 0f, Time.deltaTime * 16f);
		if ((Object)(object)Screenshotter.instance != (Object)null && Screenshotter.instance.IsScreenshotting)
		{
			FocusVolume.weight = 0f;
		}
		if ((Object)(object)WorldManager.instance.CurrentBoard != (Object)null)
		{
			MyCam.backgroundColor = WorldManager.instance.CurrentBoard.MyMaterial.GetColor("_Color");
		}
		UpdateSpiritEffect();
		prevInputCount = InputController.instance.InputCount;
	}

	public void OnRestartGame()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = (cameraTargetPosition = GetIntroCameraTransform().position);
	}

	private void UpdateSpiritEffect()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		bool inAnimation = WorldManager.instance.InAnimation;
		if ((Object)(object)WorldManager.instance.GetCard<Spirit>() != (Object)null)
		{
			flag = true;
		}
		spiritEffectStrength = Mathf.Lerp(spiritEffectStrength, (flag && inAnimation) ? 1f : 0f, Time.deltaTime * 4f);
		SpiritVolume.weight = spiritEffectStrength;
		SpiritImageEffect.Weight = spiritEffectStrength;
		Color color = TempSpiritBackgroundMaterial.color;
		color.a = spiritEffectStrength * 0.5f;
		TempSpiritBackgroundMaterial.color = color;
		if (TransitionScreen.instance.CurrentTransitionType.Id == "spirit")
		{
			SpiritTransitionEffect.Weight = TransitionScreen.instance.TransitionAmount;
		}
		else
		{
			SpiritTransitionEffect.Weight = 0f;
		}
	}

	public void Clicked()
	{
		WorldManager.instance.CloseOpenInventories();
	}

	private Vector3 GetCurrentGroundOffset()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Ray ray;
		return WorldManager.instance.ScreenPosToWorldPos(Vector2.op_Implicit(new Vector2((float)Screen.width, (float)Screen.height) * 0.5f), out ray) - ((Component)this).transform.position;
	}

	private Vector3 ClampPos(Vector3 p)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Bounds worldBounds = WorldManager.instance.CurrentBoard.WorldBounds;
		float num = Vector3.Dot(GetCurrentGroundOffset(), Vector3.forward);
		float num2 = 0.2f;
		p.x = Mathf.Clamp(p.x, ((Bounds)(ref worldBounds)).min.x - num2, ((Bounds)(ref worldBounds)).max.x + num2);
		p.z = Mathf.Clamp(p.z, ((Bounds)(ref worldBounds)).min.z - num - num2, ((Bounds)(ref worldBounds)).max.z - num + num2);
		return p;
	}
}
