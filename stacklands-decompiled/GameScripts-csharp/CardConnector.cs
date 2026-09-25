using System;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class CardConnector : Draggable
{
	private enum VisualsState
	{
		None,
		ActiveUnconnected,
		Inactive,
		ActiveConnected
	}

	[HideInInspector]
	public string UniqueId;

	[HideInInspector]
	public GameCard Parent;

	public SpriteRenderer ConnectorIcon;

	[HideInInspector]
	public string ConnectedNodeUniqueId;

	[HideInInspector]
	public CardConnector ConnectedNode;

	public Rectangle ConnectorRect;

	public Rectangle OutlineRect;

	public CardDirection CardDirection;

	public ConnectionType ConnectionType;

	[HideInInspector]
	public Vector3 Middle;

	[HideInInspector]
	public Vector3 MiddleVelo;

	public float ClosedStateScale = 0.2f;

	private bool isActive;

	private Vector3 scaleRef;

	private Vector3 targetScale;

	private Vector3 targetPosition;

	private Vector3 BasePosition;

	private VisualsState currentVisualsState;

	private static List<CardConnector> nodeTracker = new List<CardConnector>();

	public bool IsEnergyConnector
	{
		get
		{
			if (ConnectionType != ConnectionType.LV)
			{
				return ConnectionType == ConnectionType.HV;
			}
			return true;
		}
	}

	public void InitializeEnergyNode(CardConnectorData data, GameCard parent)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Parent = parent;
		CardDirection = data.EnergyConnectionType;
		ConnectionType = data.EnergyConnectionStrength;
		BasePosition = ((Component)this).transform.localPosition;
	}

	protected override void Update()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (WorldManager.instance.CurrentBoard.Id != "cities")
		{
			bool flag = false;
			if (!WorldManager.instance.CanUseTransport && ConnectionType == ConnectionType.Transport)
			{
				flag = true;
			}
			if (ConnectionType != ConnectionType.Transport)
			{
				flag = true;
			}
			if (flag)
			{
				((Component)this).transform.localScale = Vector3.zero;
				return;
			}
		}
		if (WorldManager.instance.CurrentView == ViewType.Default || WorldManager.instance.CurrentView == ViewType.Calamity)
		{
			isActive = false;
		}
		else if (WorldManager.instance.CurrentView == ViewType.Energy && ConnectionType != ConnectionType.LV && ConnectionType != ConnectionType.HV)
		{
			isActive = false;
		}
		else if (WorldManager.instance.CurrentView == ViewType.Transport && ConnectionType != ConnectionType.Transport)
		{
			isActive = false;
		}
		else if (WorldManager.instance.CurrentView == ViewType.Sewer && ConnectionType != ConnectionType.Sewer)
		{
			isActive = false;
		}
		else
		{
			isActive = true;
		}
		if (isActive)
		{
			bool flag2 = InputController.instance.StoppedGrabbing();
			if ((InputController.instance.GetInputEnded(0) || flag2) && (Object)(object)CitiesManager.instance.DrawingConnector != (Object)null)
			{
				CardConnector cardConnector = WorldManager.instance.HoveredDraggable as CardConnector;
				if ((Object)(object)cardConnector != (Object)null && (Object)(object)cardConnector != (Object)(object)CitiesManager.instance.DrawingConnector)
				{
					if ((Object)(object)cardConnector.ConnectedNode == (Object)null)
					{
						AudioManager.me.PlaySound2D(GetConnectSoundForType(ConnectionType), 1f, 0.8f);
						CitiesManager.instance.StopDrawCable(WorldManager.instance.HoveredDraggable as CardConnector);
					}
					else
					{
						CitiesManager.instance.DrawingConnector = null;
					}
				}
				else
				{
					CitiesManager.instance.StopDrawCable(null);
				}
			}
		}
		UpdateConnectorVisuals();
	}

	private Sprite GetSpriteForConnection(ConnectionType connection)
	{
		return (Sprite)(connection switch
		{
			ConnectionType.HV => SpriteManager.instance.HighVoltageSprite, 
			ConnectionType.LV => SpriteManager.instance.LowVoltageSprite, 
			ConnectionType.Sewer => SpriteManager.instance.SewerSprite, 
			ConnectionType.Transport => SpriteManager.instance.TransportSprite, 
			_ => null, 
		});
	}

	private Color GetColorForConnection(ConnectionType connection, bool isConnected)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		switch (connection)
		{
		case ConnectionType.HV:
			if (!isConnected)
			{
				return ColorManager.instance.HighVoltageConnector;
			}
			return ColorManager.instance.HighVoltageConnectorActive;
		case ConnectionType.LV:
			if (!isConnected)
			{
				return ColorManager.instance.LowVoltageConnector;
			}
			return ColorManager.instance.LowVoltageConnectorActive;
		case ConnectionType.Sewer:
			if (!isConnected)
			{
				return ColorManager.instance.SewerConnector;
			}
			return ColorManager.instance.SewerConnectorActive;
		case ConnectionType.Transport:
			if (!isConnected)
			{
				return ColorManager.instance.TransportConnector;
			}
			return ColorManager.instance.TransportConnectorActive;
		default:
			return ColorManager.instance.LowVoltageConnector;
		}
	}

	public void UpdateConnectorVisuals()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		if (!Parent.MyBoard.IsCurrent)
		{
			return;
		}
		CardConnector drawingConnector = CitiesManager.instance.DrawingConnector;
		ConnectorIcon.sprite = GetSpriteForConnection(ConnectionType);
		targetScale = Vector3.one;
		targetPosition = BasePosition;
		bool flag = (Object)(object)ConnectedNode != (Object)null;
		if (isActive)
		{
			if (!flag && currentVisualsState != VisualsState.ActiveUnconnected)
			{
				currentVisualsState = VisualsState.ActiveUnconnected;
				((ShapeRenderer)OutlineRect).Color = Color.black;
				((ShapeRenderer)ConnectorRect).Color = GetColorForConnection(ConnectionType, (Object)(object)ConnectedNode != (Object)null);
				SpriteRenderer connectorIcon = ConnectorIcon;
				Rectangle connectorRect = ConnectorRect;
				int num = (((ShapeRenderer)OutlineRect).SortingLayerID = SortingLayer.NameToID("Above"));
				int sortingLayerID = (((ShapeRenderer)connectorRect).SortingLayerID = num);
				((Renderer)connectorIcon).sortingLayerID = sortingLayerID;
				Rectangle outlineRect = OutlineRect;
				sortingLayerID = (((ShapeRenderer)ConnectorRect).RenderQueue = 3500);
				((ShapeRenderer)outlineRect).RenderQueue = sortingLayerID;
			}
			if (flag && currentVisualsState != VisualsState.ActiveConnected)
			{
				currentVisualsState = VisualsState.ActiveConnected;
				((ShapeRenderer)OutlineRect).Color = Color.black;
				((ShapeRenderer)ConnectorRect).Color = GetColorForConnection(ConnectionType, (Object)(object)ConnectedNode != (Object)null);
				SpriteRenderer connectorIcon2 = ConnectorIcon;
				Rectangle connectorRect2 = ConnectorRect;
				int num = (((ShapeRenderer)OutlineRect).SortingLayerID = SortingLayer.NameToID("Above"));
				int sortingLayerID = (((ShapeRenderer)connectorRect2).SortingLayerID = num);
				((Renderer)connectorIcon2).sortingLayerID = sortingLayerID;
				Rectangle outlineRect2 = OutlineRect;
				sortingLayerID = (((ShapeRenderer)ConnectorRect).RenderQueue = 3500);
				((ShapeRenderer)outlineRect2).RenderQueue = sortingLayerID;
			}
			PerformanceHelper.SetActive(((Component)ConnectorIcon).gameObject, active: true);
			if ((Object)(object)WorldManager.instance.HoveredDraggable == (Object)(object)this)
			{
				targetScale = Vector3.one * 1.1f;
			}
			else
			{
				targetScale = Vector3.one;
			}
			if ((Object)(object)drawingConnector != (Object)null && (Object)(object)drawingConnector != (Object)(object)this && (drawingConnector.ConnectionType != ConnectionType || drawingConnector.CardDirection == CardDirection))
			{
				targetScale = Vector3.zero;
			}
			if (IsHovered)
			{
				if (ConnectionType == ConnectionType.LV || ConnectionType == ConnectionType.HV)
				{
					string text = ((CardDirection == CardDirection.input) ? "label_connection_type_input" : "label_connection_type_output");
					string obj = ((ConnectionType == ConnectionType.LV) ? "label_connection_low_voltage" : "label_connection_high_voltage");
					GameScreen.InfoBoxText = SokLoc.Translate("label_connector_info");
					GameScreen.InfoBoxTitle = SokLoc.Translate(obj) + " " + SokLoc.Translate(text);
				}
				else if (ConnectionType == ConnectionType.Sewer)
				{
					GameScreen.InfoBoxText = SokLoc.Translate("label_connector_info");
					GameScreen.InfoBoxTitle = SokLoc.Translate("label_connection_sewer");
				}
				else if (ConnectionType == ConnectionType.Transport)
				{
					string text2 = ((CardDirection == CardDirection.input) ? "label_connection_type_input" : "label_connection_type_output");
					GameScreen.InfoBoxText = SokLoc.Translate("label_connector_info");
					GameScreen.InfoBoxTitle = SokLoc.Translate("label_connection_transport") + " " + SokLoc.Translate(text2);
				}
			}
		}
		else
		{
			SetToBackground();
		}
		((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, targetScale, 20f * Time.deltaTime);
		((Component)this).transform.localPosition = targetPosition;
	}

	private void SetToBackground()
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (currentVisualsState != VisualsState.Inactive)
		{
			currentVisualsState = VisualsState.Inactive;
			SpriteRenderer connectorIcon = ConnectorIcon;
			Rectangle connectorRect = ConnectorRect;
			int num = (((ShapeRenderer)OutlineRect).SortingLayerID = SortingLayer.NameToID("Default"));
			int sortingLayerID = (((ShapeRenderer)connectorRect).SortingLayerID = num);
			((Renderer)connectorIcon).sortingLayerID = sortingLayerID;
			Rectangle outlineRect = OutlineRect;
			sortingLayerID = (((ShapeRenderer)ConnectorRect).RenderQueue = 3000);
			((ShapeRenderer)outlineRect).RenderQueue = sortingLayerID;
			((ShapeRenderer)OutlineRect).Color = WorldManager.instance.CurrentBoard.BoardOptions.CardBackgroundPallete.Color2;
			((ShapeRenderer)ConnectorRect).Color = WorldManager.instance.CurrentBoard.BoardOptions.CardBackgroundPallete.Color;
		}
		targetScale = Vector3.one * 0.75f;
		targetPosition = BasePosition + Vector3.forward * 0.03f;
		if (Vector3.Distance(((Component)this).transform.localScale, targetScale) < 0.1f)
		{
			PerformanceHelper.SetActive(((Component)ConnectorIcon).gameObject, active: false);
		}
	}

	public override void Clicked()
	{
		if (!isActive)
		{
			return;
		}
		if ((Object)(object)ConnectedNode != (Object)null)
		{
			if ((Object)(object)CitiesManager.instance.DrawingConnector == (Object)null)
			{
				SetConnectedNode(null);
				CitiesManager.instance.StartDrawCable(this);
			}
			var (clip, vol) = GetStartSoundForType(ConnectionType);
			AudioManager.me.PlaySound2D(clip, 1f, vol);
		}
		else if ((Object)(object)CitiesManager.instance.DrawingConnector == (Object)null)
		{
			CitiesManager.instance.StartDrawCable(this);
			var (clip2, vol2) = GetStartSoundForType(ConnectionType);
			AudioManager.me.PlaySound2D(clip2, 1f, vol2);
		}
	}

	public void SetConnectedNode(CardConnector connector)
	{
		if ((Object)(object)connector != (Object)null)
		{
			ConnectedNode = connector;
			connector.ConnectedNode = this;
			return;
		}
		if ((Object)(object)ConnectedNode != (Object)null)
		{
			ConnectedNode.ConnectedNode = null;
		}
		ConnectedNode = null;
	}

	public SavedCardConnector ToSavedEnergyConnector()
	{
		if ((Object)(object)ConnectedNode == (Object)null)
		{
			return null;
		}
		return new SavedCardConnector
		{
			UniqueId = GetConnectorUniqueId(),
			ConnectedNodeUniqueId = ConnectedNode.GetConnectorUniqueId()
		};
	}

	public string GetConnectorUniqueId()
	{
		string uniqueId = Parent.CardData.UniqueId;
		string text = CardDirection.ToString();
		string text2 = ConnectionType.ToString();
		int myIndex = GetMyIndex();
		return $"{uniqueId}_{text2}_{text}_{myIndex}";
	}

	private int GetMyIndex()
	{
		int num = 0;
		for (int i = 0; i < Parent.CardConnectorChildren.Count; i++)
		{
			CardConnector cardConnector = Parent.CardConnectorChildren[i];
			if ((Object)(object)cardConnector == (Object)(object)this)
			{
				return num;
			}
			if (cardConnector.ConnectionType == ConnectionType && cardConnector.CardDirection == CardDirection)
			{
				num++;
			}
		}
		throw new Exception();
	}

	public (AudioClip, float) GetStartSoundForType(ConnectionType connection)
	{
		switch (connection)
		{
		case ConnectionType.LV:
		case ConnectionType.HV:
			return (AudioManager.me.EnergyStart, 0.6f);
		case ConnectionType.Sewer:
			return (AudioManager.me.SewerStart, 0.7f);
		case ConnectionType.Transport:
			return (AudioManager.me.TransportStart, 0.8f);
		default:
			return (null, 0f);
		}
	}

	public AudioClip GetConnectSoundForType(ConnectionType connection)
	{
		switch (connection)
		{
		case ConnectionType.LV:
		case ConnectionType.HV:
			return AudioManager.me.EnergyConnected;
		case ConnectionType.Sewer:
			return AudioManager.me.SewerConnected;
		case ConnectionType.Transport:
			return AudioManager.me.TransportConnected;
		default:
			return null;
		}
	}

	public AudioClip GetStretchSoundForType(ConnectionType connection)
	{
		switch (connection)
		{
		case ConnectionType.LV:
		case ConnectionType.HV:
			return AudioManager.me.EnergyStrech;
		case ConnectionType.Sewer:
			return AudioManager.me.SewerStrech;
		case ConnectionType.Transport:
			return AudioManager.me.TransportStrech;
		default:
			return null;
		}
	}

	public bool HasEnergyOutput()
	{
		nodeTracker.Clear();
		return Parent.CardData.HasEnergyOutput(this, nodeTracker);
	}

	public bool HasEnergyInput()
	{
		return Parent.CardData.HasEnergyInput(this);
	}

	public override bool CanBePushed()
	{
		return false;
	}

	public override bool CanBeDragged()
	{
		return false;
	}

	public override bool CanBePushedBy(Draggable draggable)
	{
		return false;
	}

	protected override void ClampPos()
	{
	}

	public GameCard GetConnectedGameCard()
	{
		return ConnectedNode?.Parent;
	}
}
