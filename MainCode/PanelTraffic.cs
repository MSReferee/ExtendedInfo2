using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework;
using ColossalFramework.Globalization;
using IINS.UIExt;
using System;
using System.IO;

namespace IINS.ExtendedInfo
{
    public class PanelTraffic : ExtendedPanel
    {
        public PanelTraffic()
        {
            name = this.GetType().Name;
            relevantComponent = parentPanel.Find("Heat'o'meter");
        }

        public override void Awake()
        {
            base.Awake();
            this.size = new Vector2(108, PANEL_HEIGHT);
            //this.eventDoubleClick += DoDoubleClick;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            DoneControls();
        }

        //public static string FormatPublicTransportCount(long s)
        //{
            //return string.Format("{0:N0}", s); //Locale.Get("INFO_PUBLICTRANSPORT_COUNT")
        //}

        public override void Start()
        {
            base.Start();
            InitControls();
            string s = "";
            s = Locale.Get("INFO_PUBLICTRANSPORT_COUNT"); // + Locale.Get("AIINFO_PASSENGERS_SERVICED") + Locale.Get("VEHICLE_PASSENGERS") + Locale.Get("TRANSPORT_LINE_PASSENGERS")
            s = s.Substring(0, s.IndexOf("{"));
            if (s.Trim() == "")
            {
                s = Locale.Get("INFO_PUBLICTRANSPORT_COUNT"); // + Locale.Get("AIINFO_PASSENGERS_SERVICED") + Locale.Get("VEHICLE_PASSENGERS") + Locale.Get("TRANSPORT_LINE_PASSENGERS")
                s = s.Substring(s.IndexOf("}") + 1);
            }
            lblPublicTransportAverage.tooltip = Locale.Get("INFO_PUBLICTRANSPORT_TITLE") + " : " + Locale.Get("INFO_PUBLICTRANSPORT_CITIZENS") + s;
        }

        public override void ResetPositionSize()
        {

            if (relevantComponent != null)
            {
                absolutePosition = relevantComponent.absolutePosition;
                if (mainAspectRatio > 0f && mainAspectRatio < 1.9f)
                    relativePosition = new Vector3(868f, 4.0f);
                else
                    relativePosition = new Vector3(relativePosition.x + 90, 4.0f);
            }
        }

        MUILabel lblIcon = null;
        MUILabel lblPublicTransportAverage = null;
        MUILabel lblVehicleCount = null;
        MUILabel lblVehicleParked = null;

        void InitControls()
        {
            LocaleManager.eventLocaleChanged += OnLocaleChanged;
            // 图标
            lblIcon = this.AddUIComponent<MUILabel>();
            lblIcon.size = new Vector2(PANEL_HEIGHT - 2, PANEL_HEIGHT - 2);
            lblIcon.relativePosition = new Vector3(2, 4);
            lblIcon.backgroundSprite = "InfoIconTrafficCongestionFocused";
            // 每周公共运输量
            lblPublicTransportAverage = this.AddUIComponent<MUILabel>();
            lblPublicTransportAverage.size = new Vector2(65, LINEW);
            lblPublicTransportAverage.relativePosition = new Vector3(36, 1);
            lblPublicTransportAverage.textColor = new Color(255, 255, 0); //Or lblPublicTransportAverage.textColor = ExtendedPanel.COLOR_TEXT;
            lblPublicTransportAverage.textAlignment = UIHorizontalAlignment.Right;
            lblPublicTransportAverage.fontStyle = FontStyle.Bold;
            lblPublicTransportAverage.fontSize = (int)MUISkin.UIToScreen(10f);

            // 车辆
            lblVehicleCount = this.AddUIComponent<MUILabel>();
            lblVehicleCount.size = new Vector2(55, LINEW);
            //lblVehicleCount.relativePosition = new Vector3(55, LINE2);
            lblVehicleCount.relativePosition = new Vector3(48, 11);
            lblVehicleCount.textColor = ExtendedPanel.COLOR_CAPTION;
            lblVehicleCount.textAlignment = UIHorizontalAlignment.Right;
            lblVehicleCount.tooltipLocaleID = "ASSETTYPE_VEHICLE";

            //已停靠車輛
            lblVehicleParked = this.AddUIComponent<MUILabel>();
            lblVehicleParked.size = new Vector2(55, LINEW);
            lblVehicleParked.relativePosition = new Vector3(48, 20);
            lblVehicleParked.textColor = new Color(255, 0, 0);//ExtendedPanel.COLOR_CAPTION;
            lblVehicleParked.textAlignment = UIHorizontalAlignment.Right;
            lblVehicleParked.tooltipLocaleID = "VEHICLE_STATUS_PARKED";

            lblIcon.eventClick += OnSetInfoModeClick;
            lblIcon.eventMouseUp += OnRightSetInfoModeClick;
            lblPublicTransportAverage.eventClick += OnSetInfoModeClick;
            lblPublicTransportAverage.eventMouseUp += OnRightSetInfoModeClick;
            lblVehicleCount.eventClick += OnSetInfoModeClick;
            lblVehicleCount.eventMouseUp += OnRightSetInfoModeClick;
            lblVehicleParked.eventClick += OnSetInfoModeClick;
            lblVehicleParked.eventMouseUp += OnRightSetInfoModeClick;

            lblIcon.playAudioEvents = true;
            lblPublicTransportAverage.playAudioEvents = true;
            lblVehicleCount.playAudioEvents = true;
            lblVehicleParked.playAudioEvents = true;
        }

        private void OnSetInfoModeClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (component == lblIcon)
            //{
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Traffic, InfoManager.SubInfoMode.Default);
                //UIView.library.Hide("TrafficInfoViewPanel");
            //}
            else if (component == lblPublicTransportAverage)
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Transport, InfoManager.SubInfoMode.Default);
            else if (component == lblVehicleCount)
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Maintenance, InfoManager.SubInfoMode.Default);
            else if (component == lblVehicleParked)
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.TrafficRoutes, InfoManager.SubInfoMode.Default);

        }

        private void OnRightSetInfoModeClick(UIComponent component, UIMouseEventParameter P)
        {
            if (P.buttons == UIMouseButton.Right)
            {
                if (component == lblIcon)
                {
                    UIView.playSoundDelegate(this.GetUIView().defaultClickSound, 1f);
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.EscapeRoutes, InfoManager.SubInfoMode.Default);
                } else
                if (component == lblPublicTransportAverage)
                {
                    UIView.playSoundDelegate(this.GetUIView().defaultClickSound, 1f);
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Wind, InfoManager.SubInfoMode.Default);
                } else
                if (component == lblVehicleCount)
                {
                    UIView.playSoundDelegate(this.GetUIView().defaultClickSound, 1f);
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.DisasterDetection, InfoManager.SubInfoMode.Default);
                } else
                if (component == lblVehicleParked)
                {
                    UIView.playSoundDelegate(this.GetUIView().defaultClickSound, 1f);
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Snow, InfoManager.SubInfoMode.Default);
                }

            }
        }

        void DoneControls()
        {
            LocaleManager.eventLocaleChanged -= OnLocaleChanged;
            Destroy(lblIcon); lblIcon = null;
            Destroy(lblPublicTransportAverage); lblPublicTransportAverage = null;
        }

        //#pragma warning disable 0108
        new void OnLocaleChanged()
        {
            
            

        }

        public override void OnScreenSizeChagned()
        {
            base.OnScreenSizeChagned();
            if (lblIcon != null)
            {
                lblIcon.size = new Vector2(PANEL_HEIGHT - 2, PANEL_HEIGHT - 2);
            }
            if (lblPublicTransportAverage != null)
            {
                lblPublicTransportAverage.fontSize = (int)MUISkin.UIToScreen(10f);
                lblPublicTransportAverage.size = new Vector2(66, LINEW);
                lblPublicTransportAverage.relativePosition = new Vector3(36, 1);
            }

            if (lblVehicleCount != null)
            {
                lblVehicleCount.size = new Vector2(55, LINEW);
                lblVehicleCount.relativePosition = new Vector3(48, 11);
                lblVehicleParked.size = new Vector2(55, LINEW);
                lblVehicleParked.relativePosition = new Vector3(48, 20);
            }
        }

        public override void UpdateData()
        {
            lblPublicTransportAverage.text = CityInfoDatas.PublicTransportAverage.text;
            lblVehicleCount.text = CityInfoDatas.VehicleCount.text;
            lblVehicleParked.text = CityInfoDatas.VehicleParked.text;


            string s = "";
            s = Locale.Get("INFO_PUBLICTRANSPORT_COUNT"); // + Locale.Get("AIINFO_PASSENGERS_SERVICED") + Locale.Get("VEHICLE_PASSENGERS") + Locale.Get("TRANSPORT_LINE_PASSENGERS")
            s = s.Substring(0, s.IndexOf("{"));
            if (s.Trim() == "")
            {
                s = Locale.Get("INFO_PUBLICTRANSPORT_COUNT"); // + Locale.Get("AIINFO_PASSENGERS_SERVICED") + Locale.Get("VEHICLE_PASSENGERS") + Locale.Get("TRANSPORT_LINE_PASSENGERS")
                s = s.Substring(s.IndexOf("}") + 1);
            }
            lblPublicTransportAverage.tooltip = Locale.Get("INFO_PUBLICTRANSPORT_TITLE") + " : " + Locale.Get("INFO_PUBLICTRANSPORT_CITIZENS") + s;
            
            lblVehicleCount.tooltipLocaleID = "ASSETTYPE_VEHICLE";
            lblVehicleParked.tooltipLocaleID = "VEHICLE_STATUS_PARKED";
        }

        public override void OnDrawPanel()
        {
            base.OnDrawPanel();
        }

        //private void DoDoubleClick(UIComponent component, UIMouseEventParameter eventParam)
        //{
        //}

        

    }
}