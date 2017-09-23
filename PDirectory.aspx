<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="PDirectory.aspx.cs" Inherits="TheBox.PDirectory" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.1/themes/base/jquery-ui.css" />
    <script type="text/javascript">
        // #region General
        var ProjectID;
        var Path = '';
        var PrivacyUsers = new Array();
        var privacyRoles = new Array();
        var Files = new Array();
        var ClickedFilePath = '';
        var ClickedFileName = '';
        var DisModal = '';
        var Filenames = new Array();
        function SetProjectID(id)
        { ProjectID = id; }

        $(document).ready(function () {
            // get the query strings
            var urlParams;
            (window.onpopstate = function () {
                var match,
                pl = /\+/g,  // Regex for replacing addition symbol with a space
                search = /([^&=]+)=?([^&]*)/g,
                decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
                query = window.location.search.substring(1);

                urlParams = {};
                while (match = search.exec(query))
                    urlParams[decode(match[1])] = decode(match[2]);
            })();

            Path = urlParams["path"];


             CKEDITOR.plugins.registered['save'] = {
         init: function (editor) 
         {
         var command = editor.addCommand('save',
         {
              modes: { wysiwyg: 1, source: 1 },
              exec: function (editor) { // Add here custom function for the save button
              var chData = CKEDITOR.instances.<%= CKEditor1.ClientID%>.getData();
              
              var encodedData = escape(chData);
              $.ajax({
                    type: "POST",
                    url: "PProject.aspx/SaveEditedFile",
                    data: '{Filepath:"' + ClickedFilePath + '", Filename:"'+ClickedFileName+'", File:"' + encodedData + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        alert('success');
                    },
                    error: function (msg, k, l) {
                        PError('Error:' + msg.responseText);
                    }
                });
              return false;
              }
         });
         editor.ui.addButton('Save', { label: 'Save', command: 'save' });     
        }  
    }
        });
        // #endregion

        function ShowFile(obj) {
            $a = $(obj);
            var path = $a.prop('href');
            if (path.split('.')[1] != 'htm') {
                ClickedFilePath = path;
                Discussions('File');
            }
            else {
                var t = $a.text();
                ClickedFileName = t;
                var spli = t.split('.');
                var newname = spli[0];
                var oladname = newname + '.htm'
                newname = newname.replace(/\ /g, '%20');
                oladname = oladname.replace(/\ /g, '%20');
                var tt = path.replace(oladname, t);
                var hfld = '<%=ConfigurationManager.AppSettings["ProjectHTMLRoot"].ToString() %>';
                var fld = '<%=ConfigurationManager.AppSettings["ProjectRoot"].ToString() %>';
                tt = tt.replace(hfld, fld);

                ClickedFilePath = tt;
                Discussions('File');
            }
            // get the comments
        }

        // #region ReadyCode
        // create new folder
        function newfolder() {
            if (document.forms[0].elements['<%=TargetFolderTag%>'].value == '') {
                alert('No folder name was provided. Enter a new folder name in the textbox.');
                document.forms[0].elements['<%=TargetFolderTag%>'].focus();
                return;
            }
            document.forms[0].action.value = 'newfolder';
            document.forms[0].submit();
        }

        function upload() {
            $('#UploadfileModal').modal('show');
            document.forms[0].action.value = 'upload';
            //document.getElementById('ctl01').submit();
            return false;
        }

        // check or uncheck all file checkboxes
        function checkall(ctl) {
            for (var i = 0; i < document.forms[0].elements.length; i++) {
                if (document.forms[0].elements[i].name.indexOf('<%=CheckboxTag%>') > -1) {
                    document.forms[0].elements[i].checked = ctl.checked;
                }
            }
        }

        // confirm file list and target folder
        function confirmfiles(sAction) {
            var nMarked = 0;
            var sTemp = '';
            for (var i = 0; i < document.forms[0].elements.length; i++) {
                if (document.forms[0].elements[i].checked &&
				 document.forms[0].elements[i].name.indexOf('<%=CheckboxTag%>') > -1) {
                    if (sAction == 'rename') {
                        var sFilename = '';
                        var sNewFilename = '';
                        sFilename = document.forms[0].elements[i].name;
                        sFilename = sFilename.replace('<%=CheckboxTag%>', '');
                        sNewFilename = prompt('Enter new name for ' + sFilename, sFilename);
                        if (sNewFilename != null) {
                            document.forms[0].elements[i].name = document.forms[0].elements[i].name + '<%=RenameTag%>' + sNewFilename;
                        }
                    }
                    nMarked = nMarked + 1;
                }
            }
            if (nMarked == 0) {
                alert('No items selected. To select items, use the checkboxes on the left.');
                return;
            }
            sTemp = 'Are you sure that you want to ' + sAction + ' the ' + nMarked + ' checked item(s)?'
            if (sAction == 'copy' || sAction == 'move') {
                sTemp = 'Are you sure you want to ' + sAction + ' the ' + nMarked + ' checked item(s) to the "' + document.forms[0].elements['<%=TargetFolderTag%>'].value + '" folder?'
                if (document.forms[0].elements['<%=TargetFolderTag%>'].value == '') {
                    document.forms[0].elements['<%=TargetFolderTag%>'].focus();
                    alert('No destination folder provided. Enter a folder name.');
                    return;
                }
            }
            var confirmed = false;
            if (sAction == 'copy' || sAction == 'rename') {
                confirmed = true;
            } else {
                confirmed = confirm(sTemp);
            }

            if (confirmed) {
                document.forms[0].action.value = sAction;
                document.forms[0].submit();
            }
        }
        preview(function () { copyText(); }, 500);

        //copy content from iframe-->textarea-->iframe
        function DoCopy(textControl, iframe01) {
            if (window.frames && window.frames[iframe01] &&
								window.frames[iframe01].document &&
								window.frames[iframe01].document.body &&
								window.frames[iframe01].document.body.innerHTML) {
                textControl.value =
			window.frames[iframe01].document.body.innerHTML;
                CKEDITOR.replace('textarea', {
                    fullPage: true,
                    allowedContent: true
                });
                CKEDITOR.instances.yourEditorInstance.commands.save.enable();
                var value = CKEDITOR.instances['iframe01'].getData();
            }
        }

        //resize iframe
        function contentFrame_onLoadClient() {
            resizeFrame(document.getElementById('<%=iframe01.ClientID %>'));
        }
        function resizeFrame(iframe01) {
            iframe01.height = iframe01.contentWindow.document.body.scrollHeight + "px";
        }
        // #endregion

        // #region ProjectDiscussion
        function Discussions(obj) {
            if (obj == 'Project') {
                if (ProjectID != 0) {
                    $('#ProjectD').show();
                    $('#FileD').hide();
                    $('#myTab a[href="#ProjectDiscussions"]').tab('show');
                    $('#iframDiv').show('slow');
                    GetDiscussions('Project');
                    return false;
                    // ajax to get the discussions
                }
            }
            else {
                $('#ProjectD').hide();
                $('#FileD').show();
                $('#iframDiv').show('slow');
                $('#myTab a[href="#View"]').tab('show');
               // GetDiscussions('File');
            }
        }

        function chkFiles(cb) {
            $c = $(cb);
            var t = $c.prop('checked');
            var name = $c.prop('name');
            name = name.replace('checked_', '');
            if (t) {
                Files.push(name);
            }
            else {
                var index = Files.indexOf(name);
                Files.splice(index, 1);
            }
        }

        function chkUsers(cb) {
            $c = $(cb);
            var t = $c.prop('checked');
            var name = $c.val();
            if (t) {
                PrivacyUsers.push(name);
            }
            else {
                var index = PrivacyUsers.indexOf(name);
                PrivacyUsers.splice(index, 1);
            }
        }

        function chkRoles(cb) {
            $c = $(cb);
            var t = $c.prop('checked');
            var name = $c.val();
            if (t) {
                privacyRoles.push(name);
            }
            else {
                var index = privacyRoles.indexOf(name);
                privacyRoles.splice(index, 1);
            }
        }

        //ajaxfileupload

        function onClientUploadComplete(sender, e) {
            Filenames.push(e.get_fileName());
        }

//        function onImageValidated(arg, context) {

//            var test = document.getElementById("testuploaded");
//            test.style.display = 'block';

//            var fileList = document.getElementById("fileList");
//            var item = document.createElement('div');
//            item.style.padding = '4px';

//            if (arg == "TRUE") {
//                var url = context.get_postedUrl();
//                url = url.replace('&amp;', '&');
//                item.appendChild(createThumbnail(context, url));
//            } else {
//                item.appendChild(createFileInfo(context));
//            }

//            fileList.appendChild(item);
//        }

//        function createFileInfo(e) {
//            var holder = document.createElement('div');
//            holder.appendChild(document.createTextNode(e.get_fileName() + ' with size ' + e.get_fileSize() + ' bytes'));

//            return holder;
//        }

//        function createThumbnail(e, url) {
//            var holder = document.createElement('div');
//            var img = document.createElement("img");
//            img.style.width = '80px';
//            img.style.height = '80px';
//            img.setAttribute("src", url);

//            holder.appendChild(createFileInfo(e));
//            holder.appendChild(img);

//            return holder;
//        }

        function onComplete() {
            var locked;
            var usernames = new Array();
            if ($('#RadioLocked').prop('checked')) {
                locked = true;
            }
            else {
                locked = false;
            }
            var privacy;
            if ($('#RadioGlobal').prop('checked')) {
                privacy = 'Global';
            }
            else if ($('#RadioPrivate').prop('checked')) {
                privacy = 'Private';
            }
            else {
                privacy = 'Public';
                $('#tdTaskNames > div').each(function (index, div) {
                    $d = $(div);
                    usernames.push($d.text());
                });
            }
            


            $.ajax({
                type: "POST",
                url: "PDirectory.aspx/SaveDirectoryFile",
                data: '{Path: "' + Path + '", Files:' + JSON.stringify(Filenames) + ', Usernames: ' + JSON.stringify(usernames) + ', Privacy:"' + privacy + '", Locked:"' + locked + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                   
                    $("#UploadfileModal").modal('hide');
                    document.forms[0].submit();
                },
                error: function () {
                    PError('Error Saving the file, Try again!');
                }
            });
        }
        function onClientUploadStart(sender, e) {
            document.getElementById('uploadCompleteInfo').innerHTML = 'Please wait while uploading ' + e.get_filesInQueue() + ' files...';
        }

        function onClientUploadCompleteAll(sender, e) {

            var privacy;
           
            if ($('#RadioGlobal').prop('checked')) {
                privacy = 'Global';
            }
            else if ($('#RadioPrivate').prop('checked')) {
                privacy = 'Private';
            }
            else {
                privacy = 'Public';
               
            }
            $.ajax({
                type: "POST",
                url: "WebForm3.aspx/onClientUploadCompleteAll",
                data: '{Path: "' + Path + '", Privacy:"' + privacy + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    //$('#UploadfileModal').modal('hide');
                    //document.forms[0].submit();
                },
                error: function () {
                    PError('Error Saving the file, Try again!');
                }
            });

            
            
//            var args = JSON.parse(e.get_serverArguments()),
//                unit = args.duration > 60 ? 'minutes' : 'seconds',
//                duration = (args.duration / (args.duration > 60 ? 60 : 1)).toFixed(2);

//            var info = 'At <b>' + args.time + '</b> server time <b>'
//                + e.get_filesUploaded() + '</b> of <b>' + e.get_filesInQueue()
//                + '</b> files were uploaded with status code <b>"' + e.get_reason()
//                + '"</b> in <b>' + duration + ' ' + unit + '</b>';

//            document.getElementById('uploadCompleteInfo').innerHTML = info;
        }
        //call function
        function hdl_change(e) {
            //document.getElementById('auto').style.visibility =
            if (e.checked)
            {
            e.id == 'MainContent_RadioPublic' ? $('#auto').show('slow') : $('#auto').hide('slow');
            var pr = $(e).val();
            $('#CheckBoxValue').val(pr);
            }
        }

        function lock_change(e)
        {
        if (e.checked)
        {
            var locked = $(e).val();
            $('#LockedValue').val(locked);
        }
        }

        function SendPrivacySettings(){
        var privacy = $('#CheckBoxValue').val();
        var locked = $('#LockedValue').val();
        var usernames = new Array();
         if (privacy == "Public")
                {
                    $('#tdTaskNames > div').each(function (index, div) {
                        $d = $(div);
                        usernames.push($d.text());
                    });
                }
                // Ajax 
                 $.ajax({
                    type: "POST",
                    url: "PDirectory.aspx/SetPrivacySettings",
                    data: '{Privacy: "' + privacy + '", Locked:"' + locked + '", Usernames: ' + JSON.stringify(usernames) + '}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function () {
                        alert('success');
                    },
                    error: function (e,r,t) {
                    //$get("<%= AjaxFileUpload1.ClientID %>_UploadOrCancelButton").click();
                    alert(e.responseText);
                    $("#MainContent_AjaxFileUpload1_FileStatusContainer").text("Error, file not uploaded");
                   
                    //document.getElementById('uploadCompleteInfo').innerHTML="Error";
                        //PError('Error sending privacy settings, Try again!');
                        
                    }
                });
        }
        
         //userpublic
        function AddTaskParti() {
            var name = $('#<%=userpublic.ClientID %>').val();
                var users = new Array();
                $('#tdTaskNames > div').each(function (index, div) {
                    $d = $(div);
                    users.push($d.text());
                });
                if ($.inArray(name, users) > -1) {
                    $('#<%=userpublic.ClientID %>').val('');
                    return false;
                }
                var txt = ' <div class="alert alert-info label-success" style="display: inline"><button type="button" class="btn btn-mini close " data-dismiss="alert"></button>' + name + '</div> ';
                var parent = $('#tdTaskNames');
                $(txt).hide().appendTo(parent).show('slow');
                $('#<%=userpublic.ClientID %>').val('');
        }

        function SaveFiles() {
       
                var strlocked = $('#LockedValaue').val();
                var privacy = $('#CheckBoxValaue').val();

                if (strlocked == 'locked') {
                    locked = true;
                }
                else {
                    locked =  false;
                }
                if (privacy == "Public")
                {
                    $('#tdTaskNames > div').each(function (index, div) {
                        $d = $(div);
                        usernames.push($d.text());
                    });
                }
               

                $.ajax({
                    type: "POST",
                    url: "PDirectory.aspx/SaveDirectoryFile",
                    data: '{Path: "' + Path + '", Files:' + JSON.stringify(Filenames) + ', Usernames: ' + JSON.stringify(usernames) + ', Privacy:"' + privacy + '", Locked:"' + locked + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        alert('success');
                    },
                    error: function () {
                        PError('Error Saving the file, Try again!');
                    }
                });
               
            
        }
        // #endregion

        // #region Edit
        function EditFile() {
            // get the content inside frame
            var text = window.frames['iframe01'].document.body.innerHTML;
            CKEDITOR.instances.<%= CKEditor1.ClientID%>.setData(text);
           
            //alert($('#iframe01').contents().find("#myContent").html());
        }
        function load() {
            CKEDITOR.instances.<%= CKEditor1.ClientID%>.setData('<p>This is the editor data.</p>');
        }
    
    // #region Upload a file
        function RefreshForm() {
        $("#UploadfileModal").modal('hide');
        document.forms[0].submit();
        }
    
    // #endregion
    </script>
    <style type="text/css">
        IMG
        {
            border: none;
        }
        BODY
        {
            font-family: Verdana, Arial, Helvetica;
            font-size: 70%;
            margin: 4px 4px 4px 4px;
        }
        TD
        {
            font-family: Verdana, Arial, Helvetica;
            font-size: 70%;
        }
        TH
        {
            font-family: Verdana, Arial, Helvetica;
            font-size: 70%;
            background-color: #eeeeee;
        }
        .Header
        {
            background-color: lightyellow;
        }
        .Error
        {
            color: red;
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager runat="Server" EnablePartialRendering="true" ID="ToolkitScriptManager1" />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <body>
                <h2>
                    Projects</h2>
                <%HandleAction();%>
                <form action="<%=ScriptName%>" method="post" enctype="multipart/form-data" data-ajax="false">
                <input type="hidden" name="<%=ActionTag%>" />
                <%  WriteTable(); %>
                <br />
                <br />
                <div id="iframDiv" hidden="hidden" style="overflow-x: hidden; overflow: auto; background-color: #ECECEE;
                    padding: 10px; border: 2px solid #CCCCCC">
                    <div class="navbar-inverse">
                        <ul id="myTab" class="nav nav-tabs " style="background-color: Black">
                            <li class="active"><a href="#View" data-toggle="tab">View</a></li>
                            <li><a href="#Edit" data-toggle="tab" onclick="EditFile()">Edit</a></li>
                            <li id="ProjectD" hidden="hidden"><a href="#ProjectDiscussions" data-toggle="tab">Project
                                Discussions</a></li>
                            <li id="FileD" hidden="hidden"><a href="#FileDiscussions" data-toggle="tab" onclick="GetDiscussions('File')">
                                File Discussions</a></li>
                        </ul>
                        <div class="tab-content" style="height: 600px">
                            <div class="tab-pane active" id="View">
                                <input type="button" class="btn pull-right" value="Download" onclick="file()" />
                                <br />
                                <iframe runat="server" scrolling="yes" frameborder="0" id='iframe01' name='iframe01'
                                    style="background-color: White; height: 560px; overflow-x: hidden;" width="100%"
                                    onload="contentFrame_onLoadServer" />
                            </div>
                            <div class="tab-pane" id="ProjectDiscussions">
                                Discussions
                                <input class="btn-mini btn-info pull-right" type="button" onclick="openDisModal()"
                                    value="Add New Discussion">
                                <hr />
                                <div class="container-fluid">
                                    <div class="row-fluid">
                                        <div style="height: 250px; overflow-x: hidden; overflow: auto; background-color: ">
                                            <br />
                                            <div id="DiscussionList" runat="server">
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div id="DiscussionPreview" class="span11">
                                            <div id="DiscussionInfo">
                                            </div>
                                            <div id="AddComment" class="container-fluid" style="display: none">
                                                <div class="row-fluid replyBox">
                                                    <div class=" replyContent">
                                                        <input type="text" id="tbDisCommentTitle" style="width: 450px" class="input-block-level"
                                                            placeholder="Title" />
                                                        <textarea id="tbDisComment" rows="0" cols="15" class="input-block-level" placeholder="Insert a comment"></textarea>
                                                        <input id="btnDisComment" type="button" value="Send" onclick="saveReply()" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="tab-pane" id="Edit">
                                <CKEditor:CKEditorControl ID="CKEditor1" BasePath="/ckeditor/" runat="server" Skin="office2003"
                                    Height="480px"></CKEditor:CKEditorControl>
                                <%--<input type="button" onclick="load()" />--%>
                            </div>
                            <div class="tab-pane" id="FileDiscussions">
                                Discussions
                                <input class="btn-mini btn-info pull-right" type="button" onclick="openDisModal('File')"
                                    value="Add New Discussion">
                                <hr />
                                <div class="container-fluid">
                                    <div class="row-fluid">
                                        <div style="height: 250px; overflow-x: hidden; overflow: auto; background-color: ">
                                            <br />
                                            <div id="FileDiscussionList" runat="server">
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div id="FileDiscussionPreview" class="span11">
                                            <div id="FileDiscussionInfo">
                                            </div>
                                            <div id="FileAddComment" class="container-fluid" style="display: none">
                                                <div class="row-fluid replyBox">
                                                    <div class=" replyContent">
                                                        <input type="text" id="FiletbDisCommentTitle" style="width: 450px" class="input-block-level"
                                                            placeholder="Title" />
                                                        <textarea id="FiletbDisComment" rows="0" cols="15" class="input-block-level" placeholder="Insert a comment"></textarea>
                                                        <input id="FilebtnDisComment" type="button" value="Send" onclick="saveFileReply()" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <br />
                <div aria-hidden="true" aria-labelledby="myModalLabel" role="dialog" tabindex="-1"
                    class="modal warning bg-color-blu hide fade" id="MyModalError" style="display: none;">
                    <div class="modal-body">
                        <p id="pError">
                        </p>
                    </div>
                    <div class="modal-footer">
                        <button data-dismiss="modal" class="btn btn-large">
                            Close</button>
                    </div>
                </div>
                <div id="UploadfileModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
                    aria-hidden="true">
                    <div class="modal-header" style="background-color: Aqua">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                        </button>
                        <h3 id="H1">
                            Upload File</h3>
                    </div>
                    <div class="modal-body" >
                        <div>
                            <div>
                                <div style="font-size: smaller;">
                                    <span style="font-size: 15px;">Privacy Setting</span><br />
                                    <input type="hidden" id="CheckBoxValue"  />
                                    <input type="hidden" id="LockedValue"  />
                                    <label class="radio inline">
                                        <input type="radio" name="optionsRadios" runat="server" id="RadioPrivate" value="Private"
                                            onchange="hdl_change(this)">
                                        <span class="metro-radio">Private</span>
                                    </label>
                                    <label class="radio inline">
                                        <input type="radio" name="optionsRadios" checked runat="server" id="RadioGlobal" value="Global" onchange="hdl_change(this)">
                                        <span class="metro-radio">Global</span>
                                    </label>
                                    <label class="radio inline">
                                        <input type="radio" name="optionsRadios" runat="server" id="RadioPublic" value="Public" onchange="hdl_change(this)">
                                        <span class="metro-radio">Public</span>
                                    </label>
                                    <hr />
                                    <label class="radio inline">
                                        <input type="radio" name="optionsRadios1" runat="server" id="RadioUnlocked" value="unlocked" onchange="lock_change(this)">
                                        <span class="metro-radio">Unlocked</span>
                                    </label>
                                    <label class="radio inline">
                                        <input type="radio" name="optionsRadios1" runat="server" id="RadioLocked" value="locked" onchange="lock_change(this)">
                                        <span class="metro-radio">Locked</span>
                                    </label>
                                   
                                </div>
                                
                                <div id="auto" hidden="hidden">
                                <hr />
                                    <table border="0" cellspacing="5px" cellpadding="5px" width="100%px">
                                        <tr>
                                            <td width="160px">
                                                <span style="font-size: 15px;">Select User :</span>
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" ID="userpublic" Width="300" autocomplete="off" Style="width: auto" />
                                                <asp:AutoCompleteExtender ID="tbAppPerson_AutoCompleteExtender" runat="server" MinimumPrefixLength="1"
                                                    ServiceMethod="GetCompletionList" TargetControlID="userpublic" UseContextKey="True" 
                                                    OnClientItemSelected="AddTaskParti">
                                                </asp:AutoCompleteExtender>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <div id="tdTaskNames">
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <hr />
                                <asp:AjaxFileUpload ID="AjaxFileUpload1" runat="server" Padding-Bottom="4" Padding-Left="2"
                                    Padding-Right="1" Padding-Top="4" OnUploadComplete="AjaxFileUpload1_OnUploadComplete"
                                    ThrobberID="myThrobber"  MaximumNumberOfFiles="1"
                                    AzureContainerName="" OnClientUploadStart="SendPrivacySettings"
                                    Style="font-size: smaller;" />
                            </div>
                        <br />
                    </div>
                    <div class="modal-footer">
                      <input type="button" onclick="RefreshForm()" value="Done"/>
                    </div>
                </div>
                </div>
                </form>
            </body>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
