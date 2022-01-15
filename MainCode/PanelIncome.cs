using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework;
using ColossalFramework.Globalization;
using IINS.UIExt;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IINS.ExtendedInfo
{
    public class PanelIncome : ExtendedPanel
    {

        public PanelIncome()
        {
            name = this.GetType().Name;
            relevantComponent = parentPanel.Find("IncomePanel");
        }
        public override void Awake()
        {
            base.Awake();
            this.size = new Vector2(265, PANEL_HEIGHT);
            //this.eventDoubleClick += DoDoubleClick;

            if (Singleton<TransferManager>.exists && m_resourceColors == null)
            {
                m_resourceColors = new Color[]
                {
                    Singleton<TransferManager>.instance.m_properties.m_resourceColors[13],
                    Singleton<TransferManager>.instance.m_properties.m_resourceColors[14],
                    Singleton<TransferManager>.instance.m_properties.m_resourceColors[0x11],
                    Singleton<TransferManager>.instance.m_properties.m_resourceColors[15],
                    Singleton<TransferManager>.instance.m_properties.m_resourceColors[0x10]
                };
            }

        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            DoneControls();
        }

        public override void Start()
        {
            base.Start();
            InitControls();
        }

        MUILabel lblIcon = null;
        MUILabel lblCashAmount = null;
        MUILabel lblCashDelta = null;
        UISprite impOilColor = null;
        UISprite impOreColor = null;
        UISprite impForestryColor = null;
        UISprite impAgricultureColor = null;
        UISprite impGoodsColor = null;
        UISprite impMailColor = null;

        MUILabel lblImportTotal = null;
        MUILabel lblImportTotalOil = null;
        MUILabel lblImportTotalOre = null;
        MUILabel lblImportTotalForestry = null;
        MUILabel lblImportTotalAgricultural = null;
        MUILabel lblImportTotalGoods = null;
        MUILabel lblImportTotalMail = null;

        UISprite expOilColor = null;
        UISprite expOreColor = null;
        UISprite expForestryColor = null;
        UISprite expAgricultureColor = null;
        UISprite expGoodsColor = null;
        UISprite expFishColor = null;
        UISprite expMailColor = null;

        MUILabel lblExportTotal = null;
        MUILabel lblExportTotalOil = null;
        MUILabel lblExportTotalOre = null;
        MUILabel lblExportTotalForestry = null;
        MUILabel lblExportTotalAgricultural = null;
        MUILabel lblExportTotalGoods = null;
        MUILabel lblExportTotalFish = null;
        MUILabel lblExportTotalMail = null;

        const int CSW = 7; // ColorSprite Width
        public static Color[] m_resourceColors = null;

        void InitControls()
        {
            LocaleManager.eventLocaleChanged += OnLocaleChanged;
            // 图标
            lblIcon = this.AddUIComponent<MUILabel>();
            lblIcon.size = new Vector2(24, 24);
            lblIcon.relativePosition = new Vector3(-2, 15);
            lblIcon.backgroundSprite = "InfoPanelIconCurrency";
            lblIcon.eventClick += OnCashAmountClick;
            lblIcon.eventMouseUp += OnRightSetInfoModeClick;
            lblIcon.playAudioEvents = true;

            // 金额
            lblCashAmount = this.AddUIComponent<MUILabel>();
            lblCashAmount.size = new Vector2(115, LINEW);
            lblCashAmount.relativePosition = new Vector3(32, LINE1);
            lblCashAmount.textColor = ExtendedPanel.COLOR_TEXT; // Or lblCashAmount.textColor = new Color(0, 255, 0);
            lblCashAmount.textAlignment = UIHorizontalAlignment.Right;
            lblCashAmount.fontStyle = FontStyle.Bold;
            lblCashAmount.fontSize = (int)MUISkin.UIToScreen(11f);
            lblCashAmount.tooltipLocaleID = "MAIN_MONEYINFO";
            lblCashAmount.eventClick += OnCashAmountClick;

            // 每周收入
            lblCashDelta = this.AddUIComponent<MUILabel>();
            lblCashDelta.size = new Vector2(105, LINEW);
            lblCashDelta.relativePosition = new Vector3(42, LINE2);
            lblCashDelta.fontSize = (int)MUISkin.UIToScreen(10f);
            lblCashDelta.textColor = COLOR_DARK_GREEN;
            lblCashDelta.fontStyle = FontStyle.Bold;
            lblCashDelta.textAlignment = UIHorizontalAlignment.Right;
            lblCashDelta.tooltipLocaleID = "MAIN_MONEYDELTA";
            lblCashDelta.eventClick += OnCashAmountClick;

            //int LOF = 153; //left offset

            // 进口
            lblImportTotal = this.AddUIComponent<MUILabel>();
            lblImportTotal.fontSize = (int)MUISkin.UIToScreen(7f);
            lblImportTotal.size = new Vector2(50, LINEW);
            lblImportTotal.relativePosition = new Vector3(158, 0);
            //lblImportTotal.relativePosition = new Vector3(153, 0);
            lblImportTotal.textColor = new Color32(255, 255, 0, 255);
            //lblImportTotal.textColor = COLOR_DARK_TEXT;
            lblImportTotal.textAlignment = UIHorizontalAlignment.Right;
            lblImportTotal.tooltipLocaleID = "INFO_CONNECTIONS_IMPORT";

            lblImportTotalOil = this.AddUIComponent<MUILabel>();
            lblImportTotalOil.fontSize = (int)MUISkin.UIToScreen(7f);
            lblImportTotalOil.size = new Vector2(50, LINEW);
            lblImportTotalOil.relativePosition = new Vector3(130, 7);
            lblImportTotalOil.textColor = new Color32(140, 120, 190, 255);
            lblImportTotalOil.textAlignment = UIHorizontalAlignment.Right;
            //lblImportTotalOil.tooltipLocaleID = "INFO_CONNECTIONS_OIL";

            lblImportTotalOre = this.AddUIComponent<MUILabel>();
            lblImportTotalOre.fontSize = (int)MUISkin.UIToScreen(7f);
            lblImportTotalOre.size = new Vector2(50, LINEW);
            lblImportTotalOre.relativePosition = new Vector3(158, 7);
            lblImportTotalOre.textColor = new Color32(0, 200, 255, 255);
            lblImportTotalOre.textAlignment = UIHorizontalAlignment.Right;
            //lblImportTotalOre.tooltipLocaleID = "INFO_CONNECTIONS_ORE";

            lblImportTotalForestry = this.AddUIComponent<MUILabel>();
            lblImportTotalForestry.fontSize = (int)MUISkin.UIToScreen(7f);
            lblImportTotalForestry.size = new Vector2(50, LINEW);
            lblImportTotalForestry.relativePosition = new Vector3(130, 14);
            lblImportTotalForestry.textColor = new Color32(0, 255, 0, 255);
            lblImportTotalForestry.textAlignment = UIHorizontalAlignment.Right;
            //lblImportTotalForestry.tooltipLocaleID = "INFO_CONNECTIONS_FORESTRY";

            lblImportTotalAgricultural = this.AddUIComponent<MUILabel>();
            lblImportTotalAgricultural.fontSize = (int)MUISkin.UIToScreen(7f);
            lblImportTotalAgricultural.size = new Vector2(50, LINEW);
            lblImportTotalAgricultural.relativePosition = new Vector3(158, 14);
            lblImportTotalAgricultural.textColor = new Color32(255, 150, 50, 255);
            lblImportTotalAgricultural.textAlignment = UIHorizontalAlignment.Right;
            //lblImportTotalAgricultural.tooltipLocaleID = "INFO_CONNECTIONS_AGRICULTURE";

            lblImportTotalGoods = this.AddUIComponent<MUILabel>();
            lblImportTotalGoods.fontSize = (int)MUISkin.UIToScreen(7f);
            lblImportTotalGoods.size = new Vector2(50, LINEW);
            lblImportTotalGoods.relativePosition = new Vector3(130, 21);
            lblImportTotalGoods.textColor = new Color32(255, 135, 255, 255);
            lblImportTotalGoods.textAlignment = UIHorizontalAlignment.Right;
            //lblImportTotalGoods.tooltipLocaleID = "INFO_CONNECTIONS_GOODS";

            lblImportTotalMail = this.AddUIComponent<MUILabel>();
            lblImportTotalMail.fontSize = (int)MUISkin.UIToScreen(7f);
            lblImportTotalMail.size = new Vector2(50, LINEW);
            lblImportTotalMail.relativePosition = new Vector3(158, 21);
            lblImportTotalMail.textColor = new Color32(0, 140, 255, 255);
            lblImportTotalMail.textAlignment = UIHorizontalAlignment.Right;
            //lblImportTotalMail.tooltipLocaleID = "INFO_CONNECTIONS_MAIL";

            impOilColor = createColorSprite(154, m_resourceColors[0], "INFO_CONNECTIONS_OIL");
            impOreColor = createColorSprite(163, m_resourceColors[1], "INFO_CONNECTIONS_ORE");
            impForestryColor = createColorSprite(172, m_resourceColors[3], "INFO_CONNECTIONS_FORESTRY");
            impAgricultureColor = createColorSprite(181, m_resourceColors[4], "INFO_CONNECTIONS_AGRICULTURE");
            impGoodsColor = createColorSprite(190, m_resourceColors[2], "INFO_CONNECTIONS_GOODS");
            impMailColor = createColorSprite(199, new Color32(0, 64, 255, 255), "INFO_CONNECTIONS_MAIL");

             
            //LOF = 213;
            // 出口
            lblExportTotal = this.AddUIComponent<MUILabel>();
            lblExportTotal.fontSize = (int)MUISkin.UIToScreen(7f);
            lblExportTotal.size = new Vector2(60, LINEW);
            lblExportTotal.relativePosition = new Vector3(200, LINE1 - 3);
            lblExportTotal.textColor = new Color32(255, 255, 0, 255);
            //lblExportTotal.textColor = COLOR_DARK_TEXT;
            lblExportTotal.textAlignment = UIHorizontalAlignment.Right;
            lblExportTotal.tooltipLocaleID = "INFO_CONNECTIONS_EXPORT";

            lblExportTotalOil = this.AddUIComponent<MUILabel>();
            lblExportTotalOil.fontSize = (int)MUISkin.UIToScreen(7f);
            lblExportTotalOil.size = new Vector2(60, LINEW);
            lblExportTotalOil.relativePosition = new Vector3(175, 5);
            lblExportTotalOil.textColor = new Color32(140, 120, 190, 255);
            lblExportTotalOil.textAlignment = UIHorizontalAlignment.Right;
            //lblExportTotalOil.tooltipLocaleID = "INFO_CONNECTIONS_OIL";

            lblExportTotalOre = this.AddUIComponent<MUILabel>();
            lblExportTotalOre.fontSize = (int)MUISkin.UIToScreen(7f);
            lblExportTotalOre.size = new Vector2(60, LINEW);
            lblExportTotalOre.relativePosition = new Vector3(200, 5);
            lblExportTotalOre.textColor = new Color32(0, 200, 255, 255);
            lblExportTotalOre.textAlignment = UIHorizontalAlignment.Right;
            //lblExportTotalOre.tooltipLocaleID = "INFO_CONNECTIONS_ORE";

            lblExportTotalForestry = this.AddUIComponent<MUILabel>();
            lblExportTotalForestry.fontSize = (int)MUISkin.UIToScreen(7f);
            lblExportTotalForestry.size = new Vector2(60, LINEW);
            lblExportTotalForestry.relativePosition = new Vector3(175, 11);
            lblExportTotalForestry.textColor = new Color32(0, 255, 0, 255);
            lblExportTotalForestry.textAlignment = UIHorizontalAlignment.Right;
            //lblExportTotalForestry.tooltipLocaleID = "INFO_CONNECTIONS_FORESTRY";

            lblExportTotalAgricultural = this.AddUIComponent<MUILabel>();
            lblExportTotalAgricultural.fontSize = (int)MUISkin.UIToScreen(7f);
            lblExportTotalAgricultural.size = new Vector2(60, LINEW);
            lblExportTotalAgricultural.relativePosition = new Vector3(200, 11);
            lblExportTotalAgricultural.textColor = new Color32(255, 150, 50, 255);
            lblExportTotalAgricultural.textAlignment = UIHorizontalAlignment.Right;
            //lblExportTotalAgricultural.tooltipLocaleID = "INFO_CONNECTIONS_AGRICULTURE";

            lblExportTotalGoods = this.AddUIComponent<MUILabel>();
            lblExportTotalGoods.fontSize = (int)MUISkin.UIToScreen(7f);
            lblExportTotalGoods.size = new Vector2(60, LINEW);
            lblExportTotalGoods.relativePosition = new Vector3(175, 17);
            lblExportTotalGoods.textColor = new Color32(255, 135, 255, 255);
            lblExportTotalGoods.textAlignment = UIHorizontalAlignment.Right;
            //lblExportTotalGoods.tooltipLocaleID = "INFO_CONNECTIONS_GOODS";

            lblExportTotalFish = this.AddUIComponent<MUILabel>();
            lblExportTotalFish.fontSize = (int)MUISkin.UIToScreen(7f);
            lblExportTotalFish.size = new Vector2(60, LINEW);
            lblExportTotalFish.relativePosition = new Vector3(200, 17);
            lblExportTotalFish.textColor = new Color32(255, 0, 0, 255);
            lblExportTotalFish.textAlignment = UIHorizontalAlignment.Right;
            //lblExportTotalFish.tooltipLocaleID = "INFO_CONNECTIONS_FISH";

            lblExportTotalMail = this.AddUIComponent<MUILabel>();
            lblExportTotalMail.fontSize = (int)MUISkin.UIToScreen(7f);
            lblExportTotalMail.size = new Vector2(60, LINEW);
            lblExportTotalMail.relativePosition = new Vector3(200, 23);
            lblExportTotalMail.textColor = new Color32(0, 140, 255, 255);
            lblExportTotalMail.textAlignment = UIHorizontalAlignment.Right;
            //lblExportTotalMail.tooltipLocaleID = "INFO_CONNECTIONS_MAIL";

            expOilColor = createColorSprite(214, m_resourceColors[0], "INFO_CONNECTIONS_OIL");
            expOreColor = createColorSprite(221, m_resourceColors[1], "INFO_CONNECTIONS_ORE");
            expForestryColor = createColorSprite(228, m_resourceColors[3], "INFO_CONNECTIONS_FORESTRY");
            expAgricultureColor = createColorSprite(235, m_resourceColors[4], "INFO_CONNECTIONS_AGRICULTURE");
            expGoodsColor = createColorSprite(242, m_resourceColors[2], "INFO_CONNECTIONS_GOODS");
            expFishColor = createColorSprite(249, new Color32(255, 64, 128, 255), "INFO_CONNECTIONS_FISH");
            expMailColor = createColorSprite(256, new Color32(0, 64, 255, 255), "INFO_CONNECTIONS_MAIL");


            impOilColor.eventClick += OnSetInfoModeClick;
            impOreColor.eventClick += OnSetInfoModeClick;
            impGoodsColor.eventClick += OnSetInfoModeClick;
            impForestryColor.eventClick += OnSetInfoModeClick;
            impAgricultureColor.eventClick += OnSetInfoModeClick;
            impMailColor.eventClick += OnSetInfoModeClick;
            lblImportTotal.eventClick += OnSetInfoModeClick;
            lblImportTotalMail.eventClick += OnSetInfoModeClick;
            impOilColor.playAudioEvents = true;
            impOreColor.playAudioEvents = true;
            impGoodsColor.playAudioEvents = true;
            impForestryColor.playAudioEvents = true;
            impAgricultureColor.playAudioEvents = true;
            impMailColor.playAudioEvents = true;
            lblImportTotal.playAudioEvents = true;
            lblImportTotalMail.playAudioEvents = true;

            expOilColor.eventClick += OnSetInfoModeClick;
            expOreColor.eventClick += OnSetInfoModeClick;
            expGoodsColor.eventClick += OnSetInfoModeClick;
            expForestryColor.eventClick += OnSetInfoModeClick;
            expAgricultureColor.eventClick += OnSetInfoModeClick;
            expFishColor.eventClick += OnSetInfoModeClick;
            expMailColor.eventClick += OnSetInfoModeClick;
            lblExportTotal.eventClick += OnSetInfoModeClick;
            lblExportTotalMail.eventClick += OnSetInfoModeClick;
            expOilColor.playAudioEvents = true;
            expOreColor.playAudioEvents = true;
            expGoodsColor.playAudioEvents = true;
            expForestryColor.playAudioEvents = true;
            expAgricultureColor.playAudioEvents = true;
            expFishColor.playAudioEvents = true;
            expMailColor.playAudioEvents = true;
            lblExportTotal.playAudioEvents = true;
            lblExportTotalMail.playAudioEvents = true;
        }

        //public class ToolsModifierControl : UICustomControl
        //{
        //    private static EconomyPanel m_EconomyPanel;
        //    public static EconomyPanel economyPanel
        //    {
        //        get
        //        {
        //            if (m_EconomyPanel == null)
        //            {
        //                UIComponent component = UIView.Find("EconomyPanel");
        //                if (component != null)
        //                {
        //                    m_EconomyPanel = component.GetComponent<EconomyPanel>();
        //                }
        //            }
        //            return m_EconomyPanel;
        //        }
        //    }

        //    public static void Show()
        //    {
        //        if (economyPanel != null)
        //        {
        //            economyPanel.Hide();
        //        }
        //    }
        //}

        private void OnSetInfoModeClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            //if (component == lblImportTotal)
            if (component == impOilColor || component == impOreColor || component == impGoodsColor || component == impForestryColor || component == impAgricultureColor || component == impMailColor || component == lblImportTotal)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Connections, InfoManager.SubInfoMode.Import);
            }
            //else if (component == lblExportTotal)
            else if (component == expOilColor || component == expOreColor || component == expGoodsColor || component == expForestryColor || component == expAgricultureColor || component == expFishColor || component == expMailColor || component == lblExportTotal)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Connections, InfoManager.SubInfoMode.Export);
            }

            else if (component == lblImportTotalMail)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Post, InfoManager.SubInfoMode.Default);
            }
            else if (component == lblExportTotalMail)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Post, InfoManager.SubInfoMode.Default);
            }
        }

        private void OnRightSetInfoModeClick(UIComponent component, UIMouseEventParameter P)
        {
            if (P.buttons == UIMouseButton.Right)
            {
                if (component == lblIcon)
                {
                    UIView.playSoundDelegate(this.GetUIView().defaultClickSound, 1f);
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.DisasterHazard, InfoManager.SubInfoMode.Default);
                }
                

            }
        }

        private void OnCashAmountClick(UIComponent com, UIMouseEventParameter p)
        {
            if (ToolsModifierControl.economyPanel != null)
            {

                if (ToolsModifierControl.economyPanel.isVisible)
                {
                    ToolsModifierControl.economyPanel.CloseToolbar(); // Hide;
                }
                else
                {
                    ToolsModifierControl.mainToolbar.ShowEconomyPanel(-1);
                    WorldInfoPanel.Hide<CityServiceWorldInfoPanel>();
                }
            }
        }

        public override void ResetPositionSize()
        {
            if (relevantComponent != null)
            {
                absolutePosition = relevantComponent.absolutePosition;
                if (mainAspectRatio > 0f && mainAspectRatio < 1.9f)
                    relativePosition = new Vector3(978f, 4.0f); // (980f, 4.0f)
                else
                    relativePosition = new Vector3(relativePosition.x + 1, 4.0f);
                    //relativePosition = new Vector3(relativePosition.x + 20, 4.0f);
            }
        }

        public UISprite createColorSprite(int left, Color32 C, string id)
        {
            UISprite result = this.AddUIComponent<UISprite>();
            result.size = new Vector2(CSW, 32);
            result.relativePosition = new Vector3(left, 1);
            result.color = C;
            result.fillDirection = UIFillDirection.Vertical;
            result.flip = UISpriteFlip.FlipVertical;
            result.fillAmount = 0.16f;
            result.opacity = 0.4f;
            result.spriteName = "EmptySprite";
            result.tooltipLocaleID = id;
            result.isTooltipLocalized = true;

            return result;
        }

        void DoneControls()
        {
            LocaleManager.eventLocaleChanged -= OnLocaleChanged;
            Destroy(lblIcon); lblIcon = null;
            Destroy(lblCashAmount); lblCashAmount = null;
            Destroy(lblCashDelta); lblCashDelta = null;
            Destroy(impOilColor); impOilColor = null;
            Destroy(impOreColor); impOreColor = null;
            Destroy(impForestryColor); impForestryColor = null;
            Destroy(impGoodsColor); impGoodsColor = null;
            Destroy(impAgricultureColor); impAgricultureColor = null;
            Destroy(impMailColor); impMailColor = null;
            Destroy(expOilColor); expOilColor = null;
            Destroy(expOreColor); expOreColor = null;
            Destroy(expForestryColor); expForestryColor = null;
            Destroy(expGoodsColor); expGoodsColor = null;
            Destroy(expAgricultureColor); expAgricultureColor = null;
            Destroy(expFishColor); expFishColor = null;
            Destroy(expMailColor); expMailColor = null;
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
                lblIcon.size = new Vector2(24, 24);
            }
            if (lblCashAmount != null)
            {
                lblCashAmount.fontSize = (int)MUISkin.UIToScreen(11f);
                lblCashAmount.size = new Vector2(115, LINEW);
                lblCashAmount.relativePosition = new Vector3(32, LINE1);
            }
            if (lblCashDelta != null)
            {
                lblCashDelta.fontSize = (int)MUISkin.UIToScreen(10f);
                lblCashDelta.size = new Vector2(105, LINEW);
                lblCashDelta.relativePosition = new Vector3(42, LINE2);
            }
            if (lblImportTotal != null)
            {
                lblImportTotal.fontSize = (int)MUISkin.UIToScreen(7f);
                lblImportTotal.size = new Vector2(50, LINEW);
                lblImportTotal.relativePosition = new Vector3(158, 0);
            }
            if (lblImportTotalOil != null)
            {
                lblImportTotalOil.fontSize = (int)MUISkin.UIToScreen(7f);
                lblImportTotalOil.size = new Vector2(50, LINEW);
                lblImportTotalOil.relativePosition = new Vector3(130, 7);
            }
            if (lblImportTotalOre != null)
            {
                lblImportTotalOre.fontSize = (int)MUISkin.UIToScreen(7f);
                lblImportTotalOre.size = new Vector2(50, LINEW);
                lblImportTotalOre.relativePosition = new Vector3(158, 7);
            }
            if (lblImportTotalForestry != null)
            {
                lblImportTotalForestry.fontSize = (int)MUISkin.UIToScreen(7f);
                lblImportTotalForestry.size = new Vector2(50, LINEW);
                lblImportTotalForestry.relativePosition = new Vector3(130, 14);
            }
            if (lblImportTotalAgricultural != null)
            {
                lblImportTotalAgricultural.fontSize = (int)MUISkin.UIToScreen(7f);
                lblImportTotalAgricultural.size = new Vector2(50, LINEW);
                lblImportTotalAgricultural.relativePosition = new Vector3(158, 14);
            }
            if (lblImportTotalGoods != null)
            {
                lblImportTotalGoods.fontSize = (int)MUISkin.UIToScreen(7f);
                lblImportTotalGoods.size = new Vector2(50, LINEW);
                lblImportTotalGoods.relativePosition = new Vector3(130, 21);
            }
            if (lblImportTotalMail != null)
            {
                lblImportTotalMail.fontSize = (int)MUISkin.UIToScreen(7f);
                lblImportTotalMail.size = new Vector2(50, LINEW);
                lblImportTotalMail.relativePosition = new Vector3(158, 21);
            }

            if (lblExportTotal != null)
            {
                lblExportTotal.fontSize = (int)MUISkin.UIToScreen(7f);
                lblExportTotal.size = new Vector2(60, LINEW);
                lblExportTotal.relativePosition = new Vector3(200, LINE1 - 3);
            }
            if (lblExportTotalOil != null)
            {
                lblExportTotalOil.fontSize = (int)MUISkin.UIToScreen(7f);
                lblExportTotalOil.size = new Vector2(60, LINEW);
                lblExportTotalOil.relativePosition = new Vector3(175, 5);
            }
            if (lblExportTotalOre != null)
            {
                lblExportTotalOre.fontSize = (int)MUISkin.UIToScreen(7f);
                lblExportTotalOre.size = new Vector2(60, LINEW);
                lblExportTotalOre.relativePosition = new Vector3(200, 5);
            }
            if (lblExportTotalForestry != null)
            {
                lblExportTotalForestry.fontSize = (int)MUISkin.UIToScreen(7f);
                lblExportTotalForestry.size = new Vector2(60, LINEW);
                lblExportTotalForestry.relativePosition = new Vector3(175, 11);
            }
            if (lblExportTotalAgricultural != null)
            {
                lblExportTotalAgricultural.fontSize = (int)MUISkin.UIToScreen(7f);
                lblExportTotalAgricultural.size = new Vector2(60, LINEW);
                lblExportTotalAgricultural.relativePosition = new Vector3(200, 11);
            }
            if (lblExportTotalGoods != null)
            {
                lblExportTotalGoods.fontSize = (int)MUISkin.UIToScreen(7f);
                lblExportTotalGoods.size = new Vector2(60, LINEW);
                lblExportTotalGoods.relativePosition = new Vector3(175, 17);
            }
            if (lblExportTotalFish != null)
            {
                lblExportTotalFish.fontSize = (int)MUISkin.UIToScreen(7f);
                lblExportTotalFish.size = new Vector2(60, LINEW);
                lblExportTotalFish.relativePosition = new Vector3(200, 17);
            }
            if (lblExportTotalMail != null)
            {
                lblExportTotalMail.fontSize = (int)MUISkin.UIToScreen(7f);
                lblExportTotalMail.size = new Vector2(60, LINEW);
                lblExportTotalMail.relativePosition = new Vector3(200, 23);
            }
        }

        decimal getFillAmount(decimal value, decimal total)
        {
            decimal v = CityInfoDatas.GetPercentValue((int)value, (int)total, true);
            return v / 100m;
        }





        public override void UpdateData()
        {
            lblCashAmount.tooltipLocaleID = "MAIN_MONEYINFO";
            lblCashDelta.tooltipLocaleID = "MAIN_MONEYDELTA";
            lblImportTotal.tooltipLocaleID = "INFO_CONNECTIONS_IMPORT";
            lblExportTotal.tooltipLocaleID = "INFO_CONNECTIONS_EXPORT";
            
            lblCashAmount.text = CityInfoDatas.CashAmount.text;
            lblCashDelta.text = CityInfoDatas.CashDelta.text;
            lblCashDelta.textColor = CityInfoDatas.CashDelta.color;

            if (lblImportTotal != null)
            {
                lblImportTotal.text = CityInfoDatas.ImportTotal.value.ToString();
                lblImportTotalOil.text = CityInfoDatas.ImportTotalOil.value.ToString();
                lblImportTotalOre.text = CityInfoDatas.ImportTotalOre.value.ToString();
                lblImportTotalForestry.text = CityInfoDatas.ImportTotalForestry.value.ToString();
                lblImportTotalAgricultural.text = CityInfoDatas.ImportTotalAgricultural.value.ToString();
                lblImportTotalGoods.text = CityInfoDatas.ImportTotalGoods.value.ToString();

                impOilColor.fillAmount = decimal.ToSingle(getFillAmount(CityInfoDatas.ImportOil.value, CityInfoDatas.ImportTotal.value));
                impOreColor.fillAmount = decimal.ToSingle(getFillAmount(CityInfoDatas.ImportOre.value, CityInfoDatas.ImportTotal.value));
                impForestryColor.fillAmount = decimal.ToSingle(getFillAmount(CityInfoDatas.ImportForestry.value, CityInfoDatas.ImportTotal.value));
                impAgricultureColor.fillAmount = decimal.ToSingle(getFillAmount(CityInfoDatas.ImportAgricultural.value, CityInfoDatas.ImportTotal.value));
                impGoodsColor.fillAmount = decimal.ToSingle(getFillAmount(CityInfoDatas.ImportGoods.value, CityInfoDatas.ImportTotal.value));

                lblExportTotal.text = CityInfoDatas.ExportTotal.value.ToString();
                lblExportTotalOil.text = CityInfoDatas.ExportTotalOil.value.ToString();
                lblExportTotalOre.text = CityInfoDatas.ExportTotalOre.value.ToString();
                lblExportTotalForestry.text = CityInfoDatas.ExportTotalForestry.value.ToString();
                lblExportTotalAgricultural.text = CityInfoDatas.ExportTotalAgricultural.value.ToString();
                lblExportTotalGoods.text = CityInfoDatas.ExportTotalGoods.value.ToString();
                lblExportTotalFish.text = CityInfoDatas.ExportTotalFish.value.ToString();

                expOilColor.fillAmount = decimal.ToSingle(getFillAmount(CityInfoDatas.ExportOil.value, CityInfoDatas.ExportTotal.value));
                expOreColor.fillAmount = decimal.ToSingle(getFillAmount(CityInfoDatas.ExportOre.value, CityInfoDatas.ExportTotal.value));
                expForestryColor.fillAmount = decimal.ToSingle(getFillAmount(CityInfoDatas.ExportForestry.value, CityInfoDatas.ExportTotal.value));
                expAgricultureColor.fillAmount = decimal.ToSingle(getFillAmount(CityInfoDatas.ExportAgricultural.value, CityInfoDatas.ExportTotal.value));
                expGoodsColor.fillAmount = decimal.ToSingle(getFillAmount(CityInfoDatas.ExportGoods.value, CityInfoDatas.ExportTotal.value));
                expFishColor.fillAmount = decimal.ToSingle(getFillAmount(CityInfoDatas.ExportFish.value, CityInfoDatas.ExportTotal.value));
            }

            if (lblImportTotalMail != null)
            {
                lblImportTotalMail.text = CityInfoDatas.ImportTotalMail.value.ToString();
                impMailColor.fillAmount = decimal.ToSingle(getFillAmount(CityInfoDatas.ImportMail.value, CityInfoDatas.ImportTotalMail.value));

                lblExportTotalMail.text = CityInfoDatas.ExportTotalMail.value.ToString();
                expMailColor.fillAmount = decimal.ToSingle(getFillAmount(CityInfoDatas.ExportMail.value, CityInfoDatas.ExportTotalMail.value));
            }
        }

        public override void OnDrawPanel()
        {
            base.OnDrawPanel(); 
            if (CanOnGUIDraw())
            {
                float W = MUISkin.UIToScreen(150);
                float T = 4f;
                float H = MUISkin.UIToScreen(PANEL_HEIGHT) - T * 2;

                GUI.DrawTexture(new Rect(W, T, 1, H), lineTexture);
                W = MUISkin.UIToScreen(210);
                GUI.DrawTexture(new Rect(W, T, 1, H), lineTexture);
            }
        }

        //private void DoDoubleClick(UIComponent component, UIMouseEventParameter eventParam)
        //{
        //}

    }
}
