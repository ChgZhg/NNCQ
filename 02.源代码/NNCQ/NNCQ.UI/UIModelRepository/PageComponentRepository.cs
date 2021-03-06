﻿using NNCQ.UI.Models;
using NNCQ.UI.Models.Web.MetroUICSS;
using NNCQ.UI.ViewModelAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Html;

namespace NNCQ.UI.UIModelRepository
{
    /// <summary>
    /// 通用的页面组件生成器
    /// </summary>
    /// <typeparam name="T">按照本框架定制的视图模型泛型</typeparam>
    public class PageComponentRepository<T> where T : class
    {
        /// <summary>
        /// 以对话框方式呈现业务对象编辑场景
        /// </summary>
        /// <param name="boVM"></param>
        /// <param name="isNew"></param>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public static MucDialogue_CreateOrEdit CreateOrEditDialog(T boVM, bool isNew) 
        {
            var preString = "编辑：";
            var preIcon = "icon-pencil";

            if (isNew) 
            {
                preString = "新建：";
                preIcon = "icon-new";
            }

            var boType = typeof(T);
            Attribute[] boVMAttributes = Attribute.GetCustomAttributes(boType);
            var editorSpecification = boVMAttributes.Where(n => n.GetType().Name == "EditorSpecification").FirstOrDefault() as EditorSpecification;

            var dialog = new MucDialogue_CreateOrEdit();
            dialog.CaptionName = preString + editorSpecification.Title;
            dialog.IconName = preIcon;
            dialog.Width = editorSpecification.Width * editorSpecification.HorizontalZoneAmount + 30;
            dialog.Height = editorSpecification.Height;

            var divName = boType.Name + editorSpecification.ControllerName;

            var htmlString = new StringBuilder();

            htmlString.Append("<script>");
            htmlString.Append("var options" + boType.Name + " = {");
            htmlString.Append("dataType: 'json',");
            htmlString.Append("success: function (data) {");

            htmlString.Append("if(data.IsOK){");
            htmlString.Append("$.Dialog.close();");
            
            htmlString.Append("if(data.ExtenssionFunctionString!=''){");
            htmlString.Append("refreshDepartmentTreeView('" + editorSpecification.ControllerName + "',data.ExtenssionFunctionString);");
            htmlString.Append("};");

            htmlString.Append("boGotoPage(data.PageIndex,'" + editorSpecification.ControllerName + "','" + editorSpecification.ListString + "',data.TypeID);");
            htmlString.Append("}else{");
            htmlString.Append("document.getElementById('Editor_" + divName + "').innerHTML = data;");
            htmlString.Append("}}");
            htmlString.Append("};");

            htmlString.Append("$('#EditorForm_" + divName + "').ajaxForm(options" + boType.Name + ");");
            htmlString.Append("</script>");

            htmlString.Append("<form action='" + editorSpecification.ControllerName + "/" + editorSpecification.SaveAction + "' method='post' id='EditorForm_" + divName + "' >");// data-ajax-update='#Editor_" + editorAttribute.ControllerName + "' data-ajax-mode='replace' data-ajax='true'>"); 

            htmlString.Append("<div id='Editor_" + divName + "' name='Editor_" + divName + "'>");
            htmlString.Append(_GetEditor(boVM));
            htmlString.Append("</div>");
            
            htmlString.Append("<div class='form-actions' style='text-align:right'><br/>");
            htmlString.Append("<input  type='submit' class='button primary' style='height:30px' value='确定' />  ");
            htmlString.Append("<button class='button' type='button' onclick='$.Dialog.close()'  style='height:30px'>取消</button>");
            htmlString.Append("</div>");
            htmlString.Append("</form>");

            dialog.InnerHtmlContent = htmlString.ToString();

            return dialog;
        }

        /// <summary>
        /// 以页面方式呈现业务对象编辑场景
        /// </summary>
        /// <param name="boVM"></param>
        /// <param name="isNew"></param>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public static MucDialogue_CreateOrEdit CreateOrEditPage(T boVM, bool isNew)
        {
            var preString = "编辑：";
            var preIcon = "icon-pencil";

            if (isNew)
            {
                preString = "新建：";
                preIcon = "icon-new";
            }

            var boType = typeof(T);
            var boName = typeof(T).Name;

            Attribute[] boVMAttributes = Attribute.GetCustomAttributes(boType);
            var editorSpecification = boVMAttributes.Where(n => n.GetType().Name == "EditorSpecification").FirstOrDefault() as EditorSpecification;

            var dialog = new MucDialogue_CreateOrEdit();
            dialog.CaptionName = preString + editorSpecification.Title;
            dialog.IconName = preIcon;
            dialog.Width = 500;
            dialog.Height = 300;

            var htmlString = new StringBuilder();
            htmlString.Append("<p class='subheader'>" + preString + editorSpecification.Title + "</p>");

            htmlString.Append("<script>");
            htmlString.Append("var options = {");
            htmlString.Append("dataType: 'json',");
            htmlString.Append("success: function (data) {");
            htmlString.Append("};");
            htmlString.Append("$('#EditorForm_" + editorSpecification.ControllerName + "').ajaxForm(options);");
            htmlString.Append("</script>");

            htmlString.Append("<form action='../../" + editorSpecification.ControllerName + "/" + editorSpecification.SaveAction + "' method='post' id='EditorForm_" + editorSpecification.ControllerName + "'data-ajax-update='#Editor_" + editorSpecification.ControllerName + "' data-ajax-mode='replace' data-ajax='true' >");// data-ajax-update='#Editor_" + editorAttribute.ControllerName + "' data-ajax-mode='replace' data-ajax='true'>"); 

            htmlString.Append("<div id='Editor_" + editorSpecification.ControllerName + "' name='Editor_" + editorSpecification.ControllerName + "'>");
            htmlString.Append(_GetEditor(boVM));
            htmlString.Append("</div>");

            htmlString.Append("<div class='form-actions' style='text-align:right'><br/>");
            htmlString.Append("<button class='button primary' style='height:30px' value=''>确定</button>");
            htmlString.Append("</div>");
            htmlString.Append("</form>");

            dialog.InnerHtmlContent = htmlString.ToString();

            return dialog;
        }

        public static MucDialogue_CreateOrEdit UpdateCreateOrEditDialog(T boVM, bool isNew,List<ValidatorResult> vItems)
        {
            var preString = "编辑：";
            var preIcon = "icon-pencil";

            if (isNew)
            {
                preString = "新建：";
                preIcon = "icon-new";
            }

            var boType = typeof(T);
            Attribute[] boVMAttributes = Attribute.GetCustomAttributes(boType);
            var editorSpecification = boVMAttributes.Where(n => n.GetType().Name == "EditorSpecification").FirstOrDefault() as EditorSpecification;

            var dialog = new MucDialogue_CreateOrEdit();
            dialog.CaptionName = preString + editorSpecification.Title;
            dialog.IconName = preIcon;
            dialog.Width = 500;
            dialog.Height = 300;

            var htmlString = new StringBuilder();

            htmlString.Append(_GetEditor(boVM, vItems));

            dialog.InnerHtmlContent = htmlString.ToString();

            return dialog;
        }

        public static MucDialogue_CreateOrEdit UpdateCreateOrEditPage(T boVM, bool isNew, List<ValidatorResult> vItems)
        {
            var preString = "编辑：";
            var preIcon = "icon-pencil";

            if (isNew)
            {
                preString = "新建：";
                preIcon = "icon-new";
            }

            var boType = typeof(T);
            Attribute[] boVMAttributes = Attribute.GetCustomAttributes(boType);
            var editorSpecification = boVMAttributes.Where(n => n.GetType().Name == "EditorSpecification").FirstOrDefault() as EditorSpecification;

            var dialog = new MucDialogue_CreateOrEdit();
            dialog.CaptionName = preString + editorSpecification.Title;
            dialog.IconName = preIcon;
            dialog.Width = 500;
            dialog.Height = 300;

            var htmlString = new StringBuilder();

            htmlString.Append(_GetEditor(boVM, vItems));

            dialog.InnerHtmlContent = htmlString.ToString();

            return dialog;
        }

        public static MucDialogue_Detail DetailDialog(T boVM) 
        {
            var boType = typeof(T);
            Attribute[] boVMAttributes = Attribute.GetCustomAttributes(boType);
            var editorSpecification = boVMAttributes.Where(n => n.GetType().Name == "EditorSpecification").FirstOrDefault() as EditorSpecification;

            var dialog = new MucDialogue_Detail();
            dialog.CaptionName = "明细数据："+editorSpecification.Title;
            dialog.Width = 500;
            dialog.Height = 300;
            dialog.InnerHtmlContent=_GetDetail(boVM);
            return dialog;

        }

        public static SaveOKStatus SaveOK(bool isOK, string pageIndex, string typeID,string extenssionFunction = null) 
        {
            var ok= new SaveOKStatus() { IsOK=isOK, PageIndex=pageIndex, TypeID=typeID };
            if (!String.IsNullOrEmpty(extenssionFunction))
                ok.ExtenssionFunctionString = extenssionFunction;

            return ok;
        }

        private static string _GetEditor(T boVM, List<ValidatorResult> vItems = null) 
        {
            var keyValue = "";
            var keyName = "";
            var editItems = new List<EditItemSpecification>();
            var boType = typeof(T);
            Attribute[] boVMAttributes = Attribute.GetCustomAttributes(boType);
            var editorAttribute = boVMAttributes.Where(n => n.GetType().Name == "EditorSpecification").FirstOrDefault() as EditorSpecification;

            PropertyInfo[] properties = boType.GetProperties();

            foreach (PropertyInfo pItem in properties) 
            {
                var keyAttibute = Attribute.GetCustomAttributes(pItem).Where(n => n.GetType().Name == "KeyAttribute").FirstOrDefault();
                if (keyAttibute != null)
                {
                    keyValue = pItem.GetValue(boVM).ToString();
                    keyName = pItem.Name;
                }

                var displayAttibute = Attribute.GetCustomAttributes(pItem).Where(n => n.GetType().Name == "DisplayAttribute").FirstOrDefault();
                if (displayAttibute != null)
                {
                    var editItem = new EditItemSpecification();
                    var displayItem = displayAttibute as System.ComponentModel.DataAnnotations.DisplayAttribute;
                    var displayName = displayItem.Name;

                    var editAtrribute = Attribute.GetCustomAttributes(pItem).Where(n => n.GetType().Name == "EditorItemSpecification").FirstOrDefault() as EditorItemSpecification;

                    if (editAtrribute != null)
                    {
                        var itemValue = pItem.GetValue(boVM);
                        editItem.FieldName = pItem.Name;
                        editItem.FieldDisplayName = displayName;
                        editItem.FieldEditContent = _GetEditFieldContent(editAtrribute.ItemType, itemValue, editItem.FieldName, boVM, vItems);
                        if (editAtrribute.ItemType == EditorItemType.Hidden) 
                        {
                            editItem.FieldDisplayName = "Hidden";
                        }
                        editItem.HorizontalZone = editAtrribute.HorizontalZone;
                        editItem.DataType = editAtrribute.ItemType;
                        editItems.Add(editItem);
                    }
                }

            }
            var maxLength = (editItems.Max(x => x.FieldDisplayName.Length)+1)*16;
            var htmlString = new StringBuilder();

            htmlString.Append("<input type='hidden' id='"+keyName+"' name='"+keyName+"' value='"+keyValue+"' />");

            foreach (var item in editItems.Where(d => d.FieldDisplayName == "Hidden"))
                htmlString.Append(item.FieldEditContent);

            var uintSpan = "span" + (12 / editorAttribute.HorizontalZoneAmount);
            htmlString.Append("<div class='grid fluid'><div class='row'>");
            for (int i = 1; i < editorAttribute.HorizontalZoneAmount+1; i++) 
            {
                htmlString.Append("<div class='" + uintSpan + "'>");
                #region 单一输入区域的组件
                htmlString.Append("<table style='width:100%'>");
                foreach (var item in editItems.Where(d => d.FieldDisplayName != "Hidden")) 
                {
                    if (item.HorizontalZone == i)
                    {
                        var v_Align = "vertical-align:center";
                        var m_String = "margin-top:-10px";
                        if (item.DataType == EditorItemType.TextArea)
                        {
                            v_Align = "vertical-align:top";
                            m_String = "";
                        }
                        htmlString.Append("<tr>");
                        htmlString.Append("<td style='width:" + maxLength + "px;text-align:right;" + v_Align + "'><div style='" + m_String + "'>" + item.FieldDisplayName + "：</div></td><td>" + item.FieldEditContent + "</td>");
                        var statusString = "<i class='icon-pencil'></>";

                        var errMessage = "";
                        if (vItems != null)
                        {
                            var eItems = vItems.Where(n => n.Name == item.FieldName);
                            if (eItems.Count() > 0)
                            {
                                foreach (var eItem in eItems)
                                {
                                    errMessage = errMessage + eItem.ErrorMessage + "\n";
                                }
                            }

                            if (!String.IsNullOrEmpty(errMessage))
                            {
                                statusString = "<i class='icon-cancel-2 fg-red'></i>";
                            }
                            else
                            {
                                statusString = "<i class='icon-checkmark fg-green'></i>";
                            }
                        }
                        htmlString.Append("<td style='width:10px;vertical-align:top'><div id='validStatus_" + item.FieldName + "'>" + statusString + "</div></td>");

                        htmlString.Append("</tr>");
                    }
                }
                htmlString.Append("</table>"); 
                #endregion
                htmlString.Append("</div>");
            }
            htmlString.Append("</div></div>");

            return htmlString.ToString();
        }

        private static string _GetEditFieldContent(EditorItemType itemType, object itemValue, string name, T boVM, List<ValidatorResult> vItems = null) 
        {
            var valueString = "";
            if (itemValue != null)
                valueString = itemValue.ToString();
            var errStyle="";
            var errMessage="";
            var errMessageStyle="";
            if (vItems != null) 
            {
                var eItems=vItems.Where(n => n.Name == name);
                if (eItems.Count() > 0) 
                {
                    foreach (var eItem in eItems) 
                    {
                        errStyle = "error-state";
                        errMessage = errMessage + eItem.ErrorMessage + "\n";
                    }
                } 
            }


            var initialStattus = "javascript:initialValidateStatus(\"css_"+name+"\",\"errorMeessage_" + name + "\",\"validStatus_" + name + "\",\"input-control text\")";
            var htmlString = new StringBuilder();
            switch (itemType)
            {
                case EditorItemType.TextBox:
                    htmlString.Append(EditorItem.A01_TextBox(name, errStyle, valueString, initialStattus, errMessage));
                    break;
                case EditorItemType.TextArea:
                    htmlString.Append(EditorItem.A02_TextArea(name, errStyle, valueString, initialStattus, errMessage, errMessageStyle));
                    break;
                case EditorItemType.DorpdownOptionWithSelfReferentialItem:
                    htmlString.Append(EditorItem.A03_DorpdownOptionWithSelfReferentialItem(boVM, name, errStyle, valueString, initialStattus, errMessage, errMessageStyle));
                    break;
                case EditorItemType.DorpdownOptionWithPlainFacadeItem:
                    htmlString.Append(EditorItem.A04_DorpdownOptionWithPlainFacadeItem(boVM, name, errStyle, valueString, initialStattus, errMessage, errMessageStyle));
                    break;
                case EditorItemType.DorpdownOptionWithEnum:
                    htmlString.Append(EditorItem.A04_DorpdownOptionWithPlainFacadeItem(boVM, name, errStyle, valueString, initialStattus, errMessage, errMessageStyle));
                   break;
                case EditorItemType.ComboBox:
                    htmlString.Append(EditorItem.A06_ComboBox(boVM, name, errStyle, valueString, initialStattus, errMessage, errMessageStyle));
                    break;
                case EditorItemType.CheckBox:
                    htmlString.Append(EditorItem.A07_CheckBox(boVM, name, errStyle, valueString, initialStattus, errMessage, errMessageStyle));
                    break;
                case EditorItemType.Date:
                    htmlString.Append(EditorItem.A05_Date(name, errStyle, valueString, initialStattus, errMessage));
                    break;
                case EditorItemType.Hidden:
                    htmlString.Append(EditorItem.A08_Hidden(name,valueString));
                    break;
                case EditorItemType.HtmlTextEditor:
                    htmlString.Append(EditorItem.A09_HtmlStringEditor(name,valueString));
                    break;
                default:
                    htmlString.Append(EditorItem.A01_TextBox(name, errStyle, valueString, initialStattus, errMessage));
                    break;
            }
            return htmlString.ToString();
        }

        private static string _GetDetail(T boVM) 
        {
            var editItems = new List<EditItemSpecification>();

            var boType = typeof(T);
            Attribute[] boVMAttributes = Attribute.GetCustomAttributes(boType);
            var editorAttribute = boVMAttributes.Where(n => n.GetType().Name == "EditorSpecification").FirstOrDefault() as EditorSpecification;

            PropertyInfo[] properties = boType.GetProperties();

            foreach (PropertyInfo pItem in properties)
            {
                var displayAttibute = Attribute.GetCustomAttributes(pItem).Where(n => n.GetType().Name == "DisplayAttribute").FirstOrDefault();
                if (displayAttibute != null)
                {
                    var editItem = new EditItemSpecification();
                    var displayItem = displayAttibute as System.ComponentModel.DataAnnotations.DisplayAttribute;
                    var displayName = displayItem.Name;

                    var detailAtrribute = Attribute.GetCustomAttributes(pItem).Where(n => n.GetType().Name == "DetailItemSpecification").FirstOrDefault() as DetailItemSpecification;

                    if (detailAtrribute != null)
                    {
                        var itemValue = pItem.GetValue(boVM);
                        editItem.FieldName = pItem.Name;
                        editItem.FieldDisplayName = displayName;
                        editItem.FieldEditContent = _GetDetailFieldContent(detailAtrribute.ItemType, itemValue);
                        //editItem.HorizontalZone=
                        editItems.Add(editItem);
                    }

                }

            }
            var maxLength = (editItems.Max(x => x.FieldDisplayName.Length) + 1) * 16;
            var htmlString = new StringBuilder();
            htmlString.Append("<table style='width:100%'>");
            foreach (var item in editItems)
            {
                htmlString.Append("<tr>");
                htmlString.Append("<td style='width:" + maxLength + "px;text-align:right;vertical-align:top'>" + item.FieldDisplayName + "：</td><td>" + item.FieldEditContent + "</td>");
                htmlString.Append("</tr>");
            }
            htmlString.Append("</table>");


            return htmlString.ToString();

        }

        private static string _GetDetailFieldContent(EditorItemType itemType, object itemValue) 
        {

            var htmlString = new StringBuilder();
            if (itemValue == null)
            {
                htmlString.Append("");
            }
            else
            {
                switch (itemType)
                {
                    case EditorItemType.TextBox:
                        htmlString.Append(itemValue.ToString());
                        break;
                    case EditorItemType.TextArea:
                        htmlString.Append(itemValue.ToString());
                        break;
                    case EditorItemType.DorpdownOptionWithSelfReferentialItem:
                        break;
                    case EditorItemType.ComboBox:
                        break;
                    case EditorItemType.Date:
                        break;
                    case EditorItemType.DateTime:
                        break;
                    case EditorItemType.Time:
                        break;
                    case EditorItemType.SelfReferentialItem:
                        var sItem = itemValue as SelfReferentialItem;
                        htmlString.Append(sItem.ItemName);
                        break;
                    case EditorItemType.PlainFacadeItem:
                        var pItem = itemValue as PlainFacadeItem;
                        htmlString.Append(pItem.Name);
                        break;
                    case EditorItemType.Hidden:
                        break;
                    case EditorItemType.HtmlTextEditor:
                        break;
                    default:
                        break;
                }
            }

            return htmlString.ToString();
        }
        
        class EditItemSpecification 
        {
            public string FieldName { get; set; }
            public string FieldDisplayName { get; set; }
            public string FieldEditContent { get; set; }
            public int HorizontalZone { get; set; }
            public EditorItemType DataType { get; set; }
        }

        class EditorItem
        {
            public static string A01_TextBox(string name, string errStyle, string valueString, string initialStattus, string errMessage) 
            {
                var htmlString = new StringBuilder();
                htmlString.Append("<div id='css_" + name + "' class='input-control text " + errStyle + "' data-role='input-control'>");
                htmlString.Append("<input id='" + name + "' name='" + name + "' type='text' placeholder='' value='" + valueString + "'  onchange='" + initialStattus + "' />");
                htmlString.Append("<button class='btn-clear' tabindex='-1'></button>");
                htmlString.Append("<div id='errorMeessage_" + name + "'><small class='fg-red'>" + errMessage + "</small></div>");
                htmlString.Append("</div>");
                return htmlString.ToString();

            }

            public static string A02_TextArea(string name, string errStyle, string valueString, string initialStattus, string errMessage, string errMessageStyle)
            {
                var htmlString = new StringBuilder();
                initialStattus = "javascript:initialValidateStatus(\"css_" + name + "\",\"errorMeessage_" + name + "\",\"validStatus_" + name + "\",\"input-control textarea\")";
                htmlString.Append("<div id='css_" + name + "' class='input-control textarea " + errStyle + "' data-role='input-control' " + errMessageStyle + " >");
                htmlString.Append("<textarea id='" + name + "' name='" + name + "' style='font-family:\"Microsoft YaHei UI\"'   onchange='" + initialStattus + "'>" + valueString + "</textarea>");
                htmlString.Append("<div id='errorMeessage_" + name + "'><small class='fg-red'>" + errMessage + "</small></div>");
                htmlString.Append("</div>");
                return htmlString.ToString();

            }

            public static string A03_DorpdownOptionWithSelfReferentialItem(T boVM, string name, string errStyle, string valueString, string initialStattus, string errMessage, string errMessageStyle)
            {
                var htmlString = new StringBuilder();
                initialStattus = "javascript:initialValidateStatus(\"css_" + name + "\",\"errorMeessage_" + name + "\",\"validStatus_" + name + "\",\"input-control select\")";
                var sItemCollection = new List<SelfReferentialItem>();
                PropertyInfo[] properties = typeof(T).GetProperties();
                foreach (var pItem in properties)
                {
                    var pAtrribute = Attribute.GetCustomAttributes(pItem).Where(n => n.GetType().Name == "SelfReferentialItemSpecification");
                    foreach (var aItem in pAtrribute)
                    {
                        var sPropertyAttribute = aItem as SelfReferentialItemSpecification;
                        if (sPropertyAttribute.RelevanceID == name)
                        {
                            sItemCollection = pItem.GetValue(boVM) as List<SelfReferentialItem>;
                        }
                    }
                }
                htmlString.Append("<div id='css_" + name + "' class='input-control select " + errStyle + "' " + errMessageStyle + " >");
                htmlString.Append("<select name='" + name + "'  onchange='" + initialStattus + "'>");
                htmlString.Append("<option value=''></option>");
                foreach (var item in sItemCollection)
                {
                    var selected = "";
                    if (item.ID == valueString)
                        selected = "selected";
                    htmlString.Append("<option " + selected + " value='" + item.ID + "'>" + item.ItemName + "</option>");
                }
                htmlString.Append("</select>");
                htmlString.Append("<div id='errorMeessage_" + name + "'><small class='fg-red'>" + errMessage + "</small></div>");
                htmlString.Append("</div>");
                return htmlString.ToString();

            }

            public static string A04_DorpdownOptionWithPlainFacadeItem(T boVM, string name, string errStyle, string valueString, string initialStattus, string errMessage, string errMessageStyle) 
            {
                var htmlString = new StringBuilder();
                initialStattus = "javascript:initialValidateStatus(\"css_" + name + "\",\"errorMeessage_" + name + "\",\"validStatus_" + name + "\",\"input-control select\")";
                var sItemCollection = new List<PlainFacadeItem>();
                PropertyInfo[] properties = typeof(T).GetProperties();
                foreach (var pItem in properties)
                {
                    var pAtrribute = Attribute.GetCustomAttributes(pItem).Where(n => n.GetType().Name == "PlainFacadeItemSpecification");
                    foreach (var aItem in pAtrribute)
                    {
                        var sPropertyAttribute = aItem as PlainFacadeItemSpecification;
                        if (sPropertyAttribute.RelevanceID == name)
                        {
                            sItemCollection = pItem.GetValue(boVM) as List<PlainFacadeItem>;
                        }
                    }
                }
                htmlString.Append("<div id='css_" + name + "' class='input-control select " + errStyle + "' " + errMessageStyle + " >");
                htmlString.Append("<select name='" + name + "'  onchange='" + initialStattus + "'>");
                htmlString.Append("<option value=''></option>");
                foreach (var item in sItemCollection)
                {
                    var selected = "";
                    if (item.ID == valueString)
                        selected = "selected";
                    htmlString.Append("<option " + selected + " value='" + item.ID + "'>" + item.Name + "</option>");
                }
                htmlString.Append("</select>");
                htmlString.Append("<div id='errorMeessage_" + name + "'><small class='fg-red'>" + errMessage + "</small></div>");
                htmlString.Append("</div>");
                return htmlString.ToString();
            }

            public static string A05_Date(string name, string errStyle, string valueString, string initialStattus, string errMessage)
            {
                var htmlString = new StringBuilder();
                htmlString.Append("<div id='dt_"+name+"' class='input-control text'>");
                htmlString.Append("<input id='" + name + "' name='" + name + "' type='text' value='" + valueString + "' readonly='readonly' onclick='javascript:getCustumerDatePicker(\"dt_"+name+"\")' />");
                htmlString.Append("</div>");
                return htmlString.ToString();
            }

            public static string A06_ComboBox(T boVM, string name, string errStyle, string valueString, string initialStattus, string errMessage, string errMessageStyle) 
            {
                var htmlString = new StringBuilder();
                initialStattus = "javascript:initialValidateStatus(\"css_" + name + "\",\"errorMeessage_" + name + "\",\"validStatus_" + name + "\",\"input-control select\")";
                var sItemCollection = new List<PlainFacadeItem>();
                PropertyInfo[] properties = typeof(T).GetProperties();
                foreach (var pItem in properties)
                {
                    var pAtrribute = Attribute.GetCustomAttributes(pItem).Where(n => n.GetType().Name == "PlainFacadeItemSpecification");
                    foreach (var aItem in pAtrribute)
                    {
                        var sPropertyAttribute = aItem as PlainFacadeItemSpecification;
                        if (sPropertyAttribute.RelevanceID == name)
                        {
                            sItemCollection = pItem.GetValue(boVM) as List<PlainFacadeItem>;
                        }
                    }
                }

                foreach (var item in sItemCollection)
                {
                    var selected = "";
                    if (item.ID.ToLower() == valueString.ToLower())
                        selected = "checked";

                    htmlString.Append("<div class='input-control radio' data-role='input-control' style='margin-right:10px;margin-top:-2px'>");
                    htmlString.Append("<label><input type='radio' value='"+item.ID+"' name='"+name+"' "+selected+" /><span class='check'></span>"+item.Name+"</label></div>");
                }
                htmlString.Append("<div id='errorMeessage_" + name + "'><small class='fg-red'>" + errMessage + "</small></div>");
                return htmlString.ToString();

            }

            public static string A07_CheckBox(T boVM, string name, string errStyle, string valueString, string initialStattus, string errMessage, string errMessageStyle)
            {
                var htmlString = new StringBuilder();
                initialStattus = "javascript:initialValidateStatus(\"css_" + name + "\",\"errorMeessage_" + name + "\",\"validStatus_" + name + "\",\"input-control select\")";
                var sItemCollection = new List<PlainFacadeItem>();
                PropertyInfo[] properties = typeof(T).GetProperties();
                foreach (var pItem in properties)
                {
                    var pAtrribute = Attribute.GetCustomAttributes(pItem).Where(n => n.GetType().Name == "PlainFacadeItemSpecification");
                    foreach (var aItem in pAtrribute)
                    {
                        var sPropertyAttribute = aItem as PlainFacadeItemSpecification;
                        if (sPropertyAttribute.RelevanceID == name)
                        {
                            sItemCollection = pItem.GetValue(boVM) as List<PlainFacadeItem>;
                        }
                    }
                }

                foreach (var item in sItemCollection)
                {
                    var selected = "";
                    if (item.ID == valueString)
                        selected = "checked";

                    htmlString.Append("<div class='input-control checkbox' data-role='input-control' style='margin-right:10px;margin-top:-2px'>");
                    htmlString.Append("<label><input type='checkbox' value='" + item.ID + "' name='" + name + "' " + selected + " /><span class='check'></span>" + item.Name + "</label></div>");
                }
                htmlString.Append("<div id='errorMeessage_" + name + "'><small class='fg-red'>" + errMessage + "</small></div>");
                return htmlString.ToString();
            }

            public static string  A08_Hidden(string name,string valueString)
            {
                var htmlString = new StringBuilder();
                htmlString.Append("<input type='hidden' value='" + valueString + "' name='" + name + "' id='" + name + "' />");
                return htmlString.ToString();
            }
            public static string A09_HtmlStringEditor(string name, string valueString)
            {
                var htmlString = new StringBuilder();
                htmlString.Append("<textarea name='" + name + "' id='" + name + "' onclick='javascript:getCustumerCkEditor(\""+name+"\")'>" + valueString + "</textarea>");
                return htmlString.ToString();
            }

            public static string A10_SinglePerson(string name, string valueString) 
            {
                var htmlString = new StringBuilder();
                htmlString.Append("<input type='hidden' id='"+name+"' name='"+name+"' value=''>");
                htmlString.Append("<table style='width:100%'>");
                htmlString.Append("<tr>");
                htmlString.Append("<td class='text-left' style='border-bottom-style:solid;border-bottom-width:1px'><div id='" + name + "Display'></div></td>");
                htmlString.Append("<td class='text-right' style='width:80px'>");
                htmlString.Append("<button class='primary' style='height:28px' onclick='javascript:getSinglePersonDialog(\"" + name + "\", \"" + name + "Display\")'> <i class='icon-checkmark'></i> 确定 </button>");
                htmlString.Append("</td>");
                htmlString.Append("</tr>");
                htmlString.Append("</table>");
                return htmlString.ToString();

            }
        }
    }

    /// <summary>
    /// 用于加载数据保存成功之后从后台返回前端的信息
    /// </summary>
    public class SaveOKStatus
    {
        public bool IsOK { get; set; }
        public string PageIndex { get; set; }
        public string TypeID { get; set; }
        public string ExtenssionFunctionString { get; set; }

        public SaveOKStatus() 
        {
            ExtenssionFunctionString = "";
        }
    }

    /// <summary>
    /// 用于加载数据删除操作完成之后返回前端的信息
    /// </summary>
    public class DeleteActionStatus
    {
        public bool IsOK { get; set; }
        public string ErrorMassage { get; set; }
        public string PageIndex { get; set; }
        public string TypeID { get; set; }
        public string ExtenssionFunctionString { get; set; }
        
        public DeleteActionStatus() 
        {
            ExtenssionFunctionString = "";
        }
    }

    public class TreeViewBuilder
    {
        private static string _Title { get; set; }
        private static List<SelfReferentialItem> _TreeNodes { get; set; }
        private string TreeViewHtmlContent { get; set; }
        private static string _TreeViewID { get; set; }
        private static string _TreeViewColID { get; set; }
        private static string _ControllerName { get; set; }
        private static string _TargetDivName { get; set; }
        private static bool _IsAjax = false;
        private static bool _IsComposite = false;
        private static bool _IsSolidHeight = true;

        public static string GetTreeViewHtmlContent(
            List<SelfReferentialItem> treeNodes,
            string title,
            string controllerName = null,
            bool? isAJAX = null,
            string divName = null,
            bool? isComposite = null,
            string divHeight = null,
            bool? isCollapsed = null,
            bool? isSolidHeight = null
            )
        {
            _ControllerName = controllerName;
            _TreeNodes = treeNodes;
            _Title = title;
            _TreeViewID = Guid.NewGuid().ToString();
            _TreeViewColID = Guid.NewGuid().ToString();
            if (isAJAX != null)
                _IsAjax = (bool)isAJAX;
            if (!String.IsNullOrEmpty(divName))
                _TargetDivName = divName;
            _IsComposite = isComposite ?? false;
            _IsSolidHeight = isSolidHeight ?? true;

            return _GetTreeViewHtmlContent(divHeight, isCollapsed);
        }

        private static string GetJScript(bool? isCollapsed = null)
        {
            var collapsed = "false";
            if (isCollapsed != null)
                collapsed = isCollapsed.ToString().ToLower();

            var htmlContent =
                "\n\n<script type='text/javascript'>" +
                "\n    $(function () {" +
                "\n        $('#" + _TreeViewID + "').treeview({" +
                "\n            collapsed: " + collapsed + ", " +
                "\n            animated: 'medium'," +
                "\n            control: '#" + _TreeViewColID + "', " +
                "\n            persist: 'location'" +
                "\n        });" +
                "\n    })" +
                "\n</script>\n\n";
            return htmlContent;
        }

        // 创建树插件
        private static string _GetTreeViewHtmlContent(string divHeight = null, bool? isCollapsed = null)
        {
            var heightString = "";
            if (!String.IsNullOrEmpty(divHeight))
                heightString = "style='height:" + divHeight + "px'";
            var treeViewContent = new StringBuilder();
            treeViewContent.Append(GetJScript(isCollapsed));
            if (!_IsComposite)
                //    treeViewContent.Append("<div class='treeheader'><h4 style='margin-top:5px;margin-left:5px'>" + _Title + " &nbsp;</h4></div>");
                //else
                treeViewContent.Append("<div class='treeheader'><h4 style='margin-top:5px;margin-left:5px'>" + _Title + " &nbsp;</h4></div>");
            treeViewContent.Append("<div id='" + _TreeViewID + "' " + heightString + ">");
            treeViewContent.Append("<ul id='tree'>");
            treeViewContent.Append(_BuildRootNodes());
            treeViewContent.Append("</ul>");
            treeViewContent.Append("</div>");
            return treeViewContent.ToString();
        }

        // 创建根节点
        private static string _BuildRootNodes()
        {
            var rootNodesContent = new StringBuilder();
            var rootNodes = _TreeNodes.Where(rn => rn.ID == rn.ParentID || rn.ParentID == "");
            foreach (var rootNode in rootNodes)
            {
                rootNodesContent.Append("<li id='" + rootNode.ID+ "'>");
                rootNodesContent.Append(GetALinkString(rootNode.OperateName, rootNode.ItemName));

                rootNodesContent.Append(_BuildSubNodes(rootNode));
                rootNodesContent.Append("</li>");
            }
            return rootNodesContent.ToString();
        }

        // 递归创建子节点
        private static string _BuildSubNodes(SelfReferentialItem rootNode)
        {
            var subNodesContent = new StringBuilder();
            var subNodes = _TreeNodes.Where(sn => sn.ParentID == rootNode.ID && sn.ID != sn.ParentID);
            if (subNodes.Count() > 0)
            {
                subNodesContent.Append("<ul>");
                foreach (var subNode in subNodes)
                {
                    subNodesContent.Append("<li id='" + subNode.ID + "'>");
                    subNodesContent.Append(GetALinkString(subNode.OperateName, subNode.ItemName));
                    subNodesContent.Append(_BuildSubNodes(subNode));
                    subNodesContent.Append("</li>");
                }
                subNodesContent.Append("</ul>");
            }
            return subNodesContent.ToString();
        }

        private static string GetALinkString(string link, string displayName)
        {
            var aLinkString = "";
            if (!String.IsNullOrEmpty(link))
            {
                if (String.IsNullOrEmpty(_ControllerName))
                {
                    if (_IsAjax)
                    {
                        aLinkString = "<a " +
                            "data-ajax='true' " +
                            //"data-ajax-confirm='确认继续操作的提示信息' " +
                            "data-ajax-method='Post' " +
                            "data-ajax-mode='replace' " +
                            "data-ajax-update='#" + _TargetDivName + "' " +
                            "href='" + link + "' " +
                            "class='itemlink'>" + displayName + "</a>";
                    }
                    else
                    {
                        aLinkString = "<a href='" + link + "ss' target='' class='itemlink'>" + displayName + "</a>";
                    }
                }
                else
                {
                    if (_IsAjax)
                    {
                        aLinkString = "<a " +
                            "data-ajax='true' " +
                            //"data-ajax-confirm='确认继续操作的提示信息' " +
                            "data-ajax-method='Post' " +
                            "data-ajax-mode='replace' " +
                            "data-ajax-update='#" + _TargetDivName + "' " +
                            "href='../../" + _ControllerName + link + "' " +
                            "class='itemlink'>" + displayName + "</a>";
                    }
                    else
                    {
                        aLinkString = "<a href='../../" + _ControllerName + link + "' target='' class='itemlink'>" + displayName + "</a>";
                    }
                }
            }
            else
                aLinkString = displayName;

            return aLinkString;
        }
    }


}
