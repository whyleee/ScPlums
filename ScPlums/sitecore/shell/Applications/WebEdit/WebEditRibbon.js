var scSavedTab = null;
var enabledSaveIcon = null;
var isRibbonVisible;
var isRibbonFixed = null;
var scShowDeletedNotification = true;
var _fix_redirecting = false;

function scSave(postaction) {
  var saveButton = $('scRibbonButton_Save');
  //Workaround for FF not appriciating disabled attribute
  if (saveButton && saveButton.disabled) {
    return;
  }
  
  Sitecore.PageEditorProxy.save(postaction);  
}

function scBeginEdit() {
  scForm.postRequest('','','','webedit:beginedit');
}

function scNew() {
  scForm.postRequest('','','','webedit:new');
}

function scNewRendering() {
  Sitecore.PageEditorProxy.showRenderingTargets();
}

function scDelete(id) {
  scForm.postRequest('','','','webedit:delete(id=' + id + ')');
}

function scSetLayout() {
  scForm.postRequest('','','','webedit:changelayout');
}

function scClose() {
  scForm.postRequest('','','','webedit:close');
}

function scSearch() {
  scForm.postRequest('','','','webedit:search');
}

function scNavigate(href) {
  //window.parent.location.href = href;
  // FIX: .href doesn't work here in FF for relative URLs for unknown reason
  // .assign() works in FF and should work in all browsers
  window.parent.location.assign(href);
  // FIX END

  // ANOTHER FIX: set flag to skip further page reload, because we want to redirect
  _fix_redirecting = true;
  // FIX END 
}

function scSetHtmlValue(controlid, preserveInnerContent, clearIfEmpty) {
  var ctl = $("scHtmlValue");
  if (ctl == null) {
    return;
  }
  
  var plainValueControl = $("scPlainValue");
  if (plainValueControl == null) {
    return;
  }
  
  var value = ctl.value;
  var plainValue = plainValueControl.value;
  
  ctl.value = "";
  plainValueControl.value = "";
  
  var win = window.parent;
  if (win == null) {
    return;
  }
 
  if (!clearIfEmpty && typeof(plainValue) != "undefined" && !plainValue.length) {
    plainValue = undefined;
  }

  Sitecore.PageEditorProxy.updateField(controlid, value, plainValue, preserveInnerContent);
}

function scSetSaveButtonState(isEnabled) {
  var scSaveButton = $("scRibbonButton_Save");
  if (scSaveButton == null) {
    return;
  }
  
  // the state is not defined. Get the persisted value from hidden input
  if (typeof(isEnabled) == "undefined") {
    isEnabled = $("__SAVEBUTTONSTATE").value == 1;
  }

  scSaveButton.disabled = !isEnabled;
  $("__SAVEBUTTONSTATE").value = isEnabled ? "1" : "0";
  if (isEnabled) {
    scSaveButton.removeClassName("scDisabledButton");
    if (typeof(disabledSaveIcon) != 'undefined') {
      var icon = scSaveButton.select("img")[0];
      if (icon != null && enabledSaveIcon != null) {
        icon.src = enabledSaveIcon;
      }
    }
  }
  else {
    scSaveButton.addClassName("scDisabledButton");
    var icon = scSaveButton.select("img")[0];    
    if (icon && typeof(disabledSaveIcon) != 'undefined') {
      if (enabledSaveIcon == null) {
        enabledSaveIcon = icon.src;
      }

      icon.src = disabledSaveIcon;
    }
  }      
}

function scShowRibbon() {  
  scSetRibbonVisibility(!isRibbonVisible);    
  var toggleIcon = $("scToggleRibbon");
  if (toggleIcon) {
    toggleIcon.src = isRibbonVisible ? 
                "/sitecore/shell/themes/standard/Images/WebEdit/more_expanded.png" :
                "/sitecore/shell/themes/standard/Images/WebEdit/more_collapsed.png";
  }
    
  scAdjustSize();  
  scForm.setCookie("sitecore_webedit_ribbon", isRibbonVisible ? "1" : "0");

  if (!isRibbonVisible) {
    scDiactiveCurrentTab();
  }    
}

function scShowPalette() {
  window.top.location.href = scSetDesigning(true);
}

function scOnLoad() { 
  scAdjustPositioning();
  var tabsContainer = $$(".scRibbonNavigatorButtonsGroupButtons");
  if (tabsContainer != null && tabsContainer.length > 0 ) {
    tabsContainer = tabsContainer[0];
    tabsContainer.childElements().each( function(tab) {
      tab.observe("click", scTabClicked);
      tab.observe("dblclick", scTabDblClicked);
    });
  }
     
  if (scIsRibbonFixed()) {    
    window.parent.$sc("<div id='scCrossPiece' style='visibility: hidden'> </div>")
      .prependTo(window.parent.document.body);    
  }
 
  scAdjustSize(); 
  Event.observe(document.body, "contextmenu", function(e) {  
    e.stop();
  });
}

function scAdjustSize() {
  var frame = scForm.browser.getFrameElement(window);    
  frame.style.height = "1px"; 
  var height = (scForm.browser.isQuirksMode ? document.body : document.documentElement).scrollHeight;
  var crossPiece = window.parent.document.getElementById("scCrossPiece");   
  if (crossPiece) {      
    crossPiece.style.height = "" + height + "px";
  }

  var frameElement = window.parent.$sc(frame);
  if (frameElement) 
  {
   if (!isRibbonVisible) {
    frameElement.addClass("scCollapsedRibbon");
   }
   else {
    frameElement.removeClass("scCollapsedRibbon");
   }

   if (scIsTreecrumbVisible() && isRibbonVisible) {
      frameElement.addClass("scTreecrumbVisible");
   }
   else
   {
      frameElement.removeClass("scTreecrumbVisible");
   }
  }

  frame.style.height = "" + height + "px";
}

if (typeof(scSitecore) != 'undefined') { 
  scSitecore.prototype.setModifiedEx = scSitecore.prototype.setModified;

  scSitecore.prototype.setModified = function (value) {
    scSetSaveButtonState(value);
    this.setModifiedEx(value);
  }

  scSitecore.prototype.onKeyDownEx = scSitecore.prototype.onKeyDown;
  scSitecore.prototype.onKeyDown = function (evt) {
      var e = (evt != null ? evt : window.event);
      if (e && e.ctrlKey && e.keyCode == 83) {
        //Ctrl + S is handled in Page Editor        
        return;
      }

      this.onKeyDownEx(evt);
   }

   scSitecore.prototype.postRequestEx = scSitecore.prototype.postRequest;
   scSitecore.prototype.postRequest = function (control, source, eventtype, parameters, callback, async) {
     var res = this.postRequestEx(control, source, eventtype, parameters, callback, async);
     if (parameters && (parameters === "item:save" || parameters.indexOf("item:save(") === 0) && res !== "failed") {
       Sitecore.PageEditorProxy.onSaving();
     }

     if (parameters && parameters === "webedit:save" && res !== "failed") {
       // FIX: do not relod the page, if we asked to redirect earlier
       if (!_fix_redirecting) {
       // FIX END
         window.parent.location.reload(true);
       }
     }

     return res;
   };
}

scRequest.prototype.buildFieldsEx = scRequest.prototype.buildFields;

scRequest.prototype.buildFields = function (doc) {
  this.buildFieldsEx(parent.document);

  this.form = this.form.replace("__VIEWSTATE=", "__VIEWSTATE2=");
  
  // Issue:311421
  this.form = this.form.replace("__PREVIOUSPAGE=", "__PREVIOUSPAGE2=");
  
  this.buildFieldsEx();
}

scContentEditor.prototype.setActiveStripEx = scContentEditor.prototype.setActiveStrip;

scContentEditor.prototype.setActiveStrip = function(id, toggleRibbon) {
  scContentEditor.prototype.setActiveStripEx(id, toggleRibbon);
  
  var ctl = scForm.browser.getControl("scActiveRibbonStrip");
  
  if (ctl != null) {
    scForm.setCookie("sitecore_webedit_activestrip", ctl.value);
  }

  scFixIeMinMaxWidth();
}

scContentEditor.prototype.showGallery = function(sender, evt, id, src, parameters, width, height, where) {
  this.showOutOfFrameGallery(sender, evt, src, {width:  width, height: height },  parameters);
}

function scSetDesigning(enabled) {
  var page = window.top.location.href;
  
  var params = page.toQueryParams();
  
  if (enabled) {
    params["sc_de"] = "1";
  }
  else {
    delete params["sc_de"];
  }
  
  var n = page.indexOf("?");
  if (n >= 0) {
    page = page.substr(0, n + 1);
  }
  else {
    page += "?";
  }

  return page + Object.toQueryString(params);
}

function scSetModified(modified) {
  var doc = scForm.browser.getParentWindow(scForm.browser.getFrameElement(window).ownerDocument);

  if (doc.Sitecore.WebEdit) {
    doc.Sitecore.WebEdit.modified = modified;
  }
  else {
    doc.Sitecore.Designer.setModified(modified);
  }
}

function scTabClicked() {      
  scSavedTab = null;
  if (!isRibbonVisible) {
     var param = new Object();
     param.target = $$("a.scToggleIcon img")[0];
     scShowRibbon(param);
  }
}

function scTabDblClicked(e) { 
  if (isRibbonVisible) {
     var param = new Object();
     param.target = $$("a.scToggleIcon img")[0];
     scShowRibbon(param);
  }
}

function scSetRibbonVisibility(visibility) {
  var ribbon = $("Ribbon");
  if (ribbon.down() && ribbon.down().hasClassName("scRibbonNavigator")) {
    ribbon = $("Ribbon_Toolbar");    
  }
  else {
    ribbon = $("RibbonPane");    
  }

  var treeCrumb = $("TreecrumbPane");
  var notifications = $("NotificationPane");
  isRibbonVisible = visibility;
  if (isRibbonVisible) {
    ribbon.show();
    if (scIsTreecrumbVisible()) {
      treeCrumb.show();
    }
    notifications.show();
    if (scSavedTab != null) {
      scSavedTab.className = "scRibbonNavigatorButtonsActive";
    }
  }
  else {
    ribbon.hide();
    treeCrumb.hide();
    notifications.hide();
  }
}

function scDiactiveCurrentTab() {
  var activeTabs = $$(".scRibbonNavigatorButtonsActive");
  if (activeTabs != null && activeTabs.length > 0) {      
    activeTabs[0].className = "scRibbonNavigatorButtonsNormal";
    scSavedTab = activeTabs[0];
  }
}

function scAdjustPositioning() {
  var buttonsContainer = $("Buttons");
  if (buttonsContainer == null) return;
  var commands = buttonsContainer.select(".scCommandIcon, .scMenuDevider");
  var commandWidth = 0;
  if (commands.length > 0) {
    commandWidth = commands[0].measure("margin-box-width");      
  }

  var deviders = buttonsContainer.select(".scMenuDevider");
  var deviderWidth = 0;
   if (deviders.length > 0) {
    deviderWidth = deviders[0].measure("margin-box-width");      
  }

  var ribbonNavigators = $$(".scRibbonNavigator");
  //Set the navigator margin according to the number of commands
  if (ribbonNavigators.length > 0 && ribbonNavigators[0].visible()) {
    ribbonNavigators[0].setStyle({ marginLeft: commands.length * commandWidth + deviders.length * deviderWidth + 12 + "px" });
  }
  // There's no ribbon tabs. Add the margin to the toolbar to avoid overlaping with commands
  else if (buttonsContainer.childElements().length > 0) {
    var ribbonPanel = $("RibbonPanel");
    if (ribbonPanel) {
      var marginValue = buttonsContainer.measure("border-box-height");
      if (marginValue >= 1 && Prototype.Browser.IE) {
        marginValue -= 1;
      }

      ribbonPanel.setStyle({ marginTop: marginValue + "px" });
    }
  }

  scFixIeMinMaxWidth();
}

function scFixIeMinMaxWidth() {
  if (!scForm.browser.isIE) {
    return;
  }

  var elementsToFix = $$(".scFixIeMinMaxWidth");
  elementsToFix.each(function(elem) {
    var maxWidth = parseInt(elem.getStyle("max-width"));
    var minWidth = parseInt(elem.getStyle("min-width"));
    maxWidth = maxWidth ? maxWidth : 9999;
    minWidth = minWidth ? minWidth : 0;
    var width = elem.getWidth();
    if (width > 0) {
      elem.removeClassName("scFixIeMinMaxWidth");
    }

    if (width > maxWidth) {
      elem.setStyle({ width: maxWidth + "px" });
      return;
    }

    if (width < minWidth) {
      elem.setStyle({ width: minWidth + "px" });
    }
  });
}

function scShowControlsClick(enabled) {  
  Sitecore.PageEditorProxy.changeShowControls(enabled);
  // enforce ribbon refresh
    scRefreshRibbon();
}

function scCapabilityClick(capability, enabled) {    
  Sitecore.PageEditorProxy.changeCapability(capability, enabled);
  // enforce ribbon refresh to make capabilty button have appropriate state
    scRefreshRibbon();
}

function scShowTreecrumb(isTreecrumbVisisble)
{ 
  if (!isTreecrumbVisisble) {   
    scTreecrumbVisible = false;
    $("TreecrumbPane").hide();    
  }
  else {
    scTreecrumbVisible = true;    
    $("TreecrumbPane").show();
  }

  // enforce ribbon refresh
    scRefreshRibbon();
  scAdjustSize();
}


function scToggleControlBar(isVisible) {
  scControlBar = isVisible;    
  Sitecore.PageEditorProxy.controlBarStateChange();
   // enforce ribbon refresh
    scRefreshRibbon();
}

function scIsRibbonFixed() {
  if (isRibbonFixed == null) {
    var frame = window.parent.$sc("#scWebEditRibbon")
    if (frame && frame.length > 0) {
      isRibbonFixed = frame.hasClass("scFixedRibbon");
    }
  }

  return isRibbonFixed;
}

function scIsTreecrumbVisible() {
  return typeof(scTreecrumbVisible) != 'undefined' && scTreecrumbVisible == true;
}

function scShowComponentsGallery(sender, e, galleryName, dimensions, params) {
  var layout = Sitecore.PageEditorProxy.layout();
  var deviceID = Sitecore.PageEditorProxy.deviceId();
  if (!params) {
    params = {};
  }

  params.layout = layout;
  params.device = deviceID;
 
  params.isTestRunning = Sitecore.PageEditorProxy.isTestRunning();
  return scContent.showOutOfFrameGallery(sender,e, galleryName, dimensions, params, "POST");
}

function scShowItemDeletedNotification(text) {
  if (!window.scShowDeletedNotification) {
    return;
  }

  var win = window.parent;
  var notificaton = new win.Sitecore.PageModes.Notification("itemdeleted", text, {
    type: "error"
  });

  Sitecore.PageEditorProxy.showNotification(notificaton);
  window.scShowDeletedNotification = false; 
}

function scRefreshRibbon() {
  scForm.postRequest("", "", "", "ribbon:update", null, true);
}