<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="PProject.aspx.cs" Inherits="TheBox.PProject" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.1/themes/base/jquery-ui.css" />
    <script type="text/javascript" src="content/js/jquery.cookie.js"></script>
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
        var _EditTask = 0;
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
         init: function (editor) {
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
                        alert('File Saved!');
                    },
                    error: function (msg, sta, ex) {
                        var r = jQuery.parseJSON(msg.responseText);
                        PError('Error:' + r.Message);
                    }
                });
              return false;
              }
         });
         editor.ui.addButton('Save', { label: 'Save', command: 'save' });
      }
  }
  
        });
        
        function formatDate(d) {
            var date = new Date(parseInt(d.substr(6)))
            return date.getMonth() + 1 + "-" + date.getDate() + "-" + date.getFullYear();
        }
        function formatTime(date) {
            var h = date.getHours();
            var m = date.getMinutes();
            var s = date.getSeconds();
            if (h < 10) h = '0' + h;
            if (m < 10) m = '0' + m;
            if (s < 10) s = '0' + s;

            return h + ":" + m + ":" + s;
        }
        
         function ConfirmTask(hd, msg, id) {
            $('#btnConfirm').attr('onclick', '');
            $('#btnConfirm').unbind('click');
            $('#btnConfirm').click(function () {
                DeleteFileTask(id);
            });
            $('#PartiModal').modal('show');
            $('#txtConfirm').html(msg);
            $('#headerConfirm').text(hd);
        }
        // #endregion

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
            if (document.forms[0].elements['fileupload'].value == '') {
                alert('No upload file was provided. Select a file to upload.');
                document.forms[0].elements['fileupload'].focus();
                return;
            }
            document.forms[0].action.value = 'upload';

            document.getElementById('ctl01').submit();
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

        // #region Project Functions
        function NewProject() {
            $('#NewProjectModal').modal('show');
            return false;
        }

        function PError(msg) {
            $('#pError').text(msg);
            $('#MyModalError').modal('show');
        }
        // create new project
        function newProject() {
            if ($('#tbProjectName').val() == "") {
                return false;
            }
            document.forms[0].action.value = 'newproject';
            document.forms[0].submit();
            $('#NewProjectModal').modal('hide');
        }

        function AddProjectUser() {
            var user = $('#<%=lbUsers.ClientID %> option:selected').val();
            var role = $('#<%=lbRole.ClientID %> option:selected').val();

            if (role == undefined || user == undefined) {
                alert('You must choose a user and a role');
                return;
            }
            var list = GetAddedUserNames();
            // check username
            //$.inArray(user, list)
            if (list.indexOf(user) > -1) {
                alert('The user already among participants');
                return false;
            }
            var tr = '<tr><td>' + user + '</td><td>' + role + '</td><td><a href="#" onclick="DeleteUser(this, "")"><span class="icon-x"></span></a></td></tr>';
            $('#tblUsers > tbody:last').append(tr);
            var temp = $('#<%= hiddenUsers.ClientID%>').val();
            if (temp == undefined) {
                $('#<%= hiddenUsers.ClientID%>').val(user + '.' + role + '|');
            }
            else {
                $('#<%= hiddenUsers.ClientID%>').val(temp + user + '.' + role + '|');
            }

            return false;
        }

        function GetAddedUserNames() {
            var UserList = new Array();
            $('#tblUsers  tr').each(function (index, tr) {
                var $row = $(tr);

                var txt = $row.find('td:nth-child(1)').text();
                if (txt != '') {
                    UserList.push(txt);
                }
            });
            return UserList;
        }

        function DeleteUser(obj, str) {
            if (str == 'Edit') {
                if (confirm('Are you sure you want to delete the user?')) {
                    //get username
                    var $tr = $(obj).closest('tr');
                    var username = $tr.find('td:nth-child(1)').text();
                    if (ProjectID != 0) {
                        $.ajax({
                            type: "POST",
                            url: "PProject.aspx/DeleteProjectUser",
                            data: '{ProjectID:"' + ProjectID + '", Username:"' + username + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (response) {
                                if (response.d == 'true') {
                                    $tr.remove();
                                }
                            },
                            error: function () {
                                PError('The user is creator of one of the project files, Try again!');
                            }
                        });
                        return false;
                    }
                }
                else {
                    alert('canceled');
                }
            }
            else {
                var $tr = $(obj).closest('tr');
                var user = $tr.find('td:nth-child(1)').text();
                var role = $tr.find('td:nth-child(2)').text();
                var str = user + '.' + role + '|';
                var hid = $('#<%=hiddenUsers.ClientID%>').val();
                hid = hid.replace(str, '');
                $('#<%=hiddenUsers.ClientID%>').val(hid);
                $tr.fadeOut(400, function () {
                    $tr.remove();
                });
                return false;
            }
        }

        function EditProjectUsers() {

            if (ProjectID != 0) {

                $.ajax({
                    type: "POST",
                    url: "PProject.aspx/GetProjectUsers",
                    data: '{projectid:"' + ProjectID + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {

                        $('#modalProjectName').hide();
                        $('#tblUsers').find("tr:gt(0)").remove();
                        $('#tblUsers > tbody:last').append(response.d);
                        $('#tbProjectNameID').val('EditUsers|' + ProjectID);
                        $('#NewProjectModal').modal('show');
                        //$('#tbProjectNameID').hide();

                    },
                    error: function () {
                        PError('Error fetching users, Try again!');
                    }
                });
                return false;
            }
            // open modal then Ajax, fill data

        }

        function ShowPrivacyUsers(str) {
            switch (str) {
                case "User":
                    $('#divPrivacyUsers').show('slow');
                    $('#divPrivacyRole').hide('slow');
                    $.ajax({
                        type: "POST",
                        url: "PProject.aspx/GetProjectUsernames",
                        data: '{projectid:"' + ProjectID + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            $('#divPrivacyUsers').html(response.d);

                        },
                        error: function () {
                            PError('Error fetching users, Try again!');
                        }
                    });

                    break;
                case "Role":
                    $('#divPrivacyUsers').hide('slow');
                    $('#divPrivacyRole').show('slow');
                    break;

            }

        }


        function ShowPrivacyBy(str) {
            switch (str) {
                case "Public":
                    $('#divPrivacyPrivate').hide('slow');
                    $('#divPrivacyUsers').hide('slow');
                    $('#divPrivacyRole').hide('slow');
                    break;
                case "Private":
                    $('#divPrivacyPrivate').show('slow');
                    $('#divPrivacyUsers').hide('slow');
                    $('#divPrivacyRole').hide('slow');
                    break;
            }
        }

        function ProjectFilePrivacy() {
        $('#divPrivacyFiles1').html('');

            if (Files.length == 0) {
                $('#divPrivacyFiles1').append('No Files Selected');
                return false;
            }
            else {
                var file = Files[0].split('.')[1];
                if (Files.length == 1 && ProjectID != 0 && file != undefined) {
                    // call the get
                    $.ajax({
                        type: "POST",
                        url: "PProject.aspx/GetProjectFilePrivacy",
                        data: '{ProjectID:"' + ProjectID + '", Filename:"' + Files[0] + '", Dir:"' + Path + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            var obj = response.d;
                            $('#divPrivacyFiles1').append(Files[0] + '</br>');
                            if (obj.Public) {
                                ShowPrivacyBy('Public');
                                $('#RadioPublic').prop('checked', true);
                            }
                            else {
                                ShowPrivacyBy('Private');
                                $('#RadioPrivate').prop('checked', true);
                                if (obj.By == "User") {
                                    $('#divPrivacyUsers').show('slow');
                                    $('#divPrivacyRole').hide('slow');
                                    $('#RadioUser').prop('checked', true);
                                    $('#divPrivacyUsers').html(obj.UsernamesString);
                                    PrivacyUsers = new Array();
                                    for (var i = 0; i < obj.Usernames.length; i++) {
                                        PrivacyUsers.push(obj.Usernames[i]);
                                    }
                                }
                                else {
                                    $('#divPrivacyUsers').hide('slow');
                                    $('#divPrivacyRole').show('slow');
                                    $('#RadioRole').prop('checked', true);
                                    privacyRoles = new Array();
                                    $('#divPrivacyRole').find(':checkbox').each(function(){
                                    //$('#divPrivacyRole > input[type = checkbox]').each(function (i,o) {
                                    var tt = $(this).val();
                                    var arr = $.makeArray(obj.PRoles);
                                    var b = $.inArray(tt, arr);
                                    if(b!= -1)
                                    {
                                         $(this).prop('checked', true);
                                         privacyRoles.push(obj.PRoles[t]);
                                    }
                                    else
                                    {
                                        $(this).prop('checked', false);
                                    }
                                    });
                                }
                            }
                            $('#PrivacyModal').modal('show');
                            return false;
                            },
                        error: function (msg, sta, ex) {
                        var r = jQuery.parseJSON(msg.responseText);
                        PError('Error:' + r.Message);
                        return false;
                        }
                    });
                }
                else {
                    for (var t = 0; t < Files.length; t++) {
                        $('#divPrivacyFiles1').append(Files[t] + '</br>');
                    }
                    $('#PrivacyModal').modal('show');
                    return false;
                }
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
        
        function GetRoles()
        {
            privacyRoles = new Array();
            $('#divPrivacyRole').find(':checkbox').each(function(){
            if ($(this).prop('checked'))
            {
                var role = $(this).val();
                privacyRoles.push(role);
            }
            });
        }

        function SaveProjectPrivacy() {
            // get files
            if (Files.length == 0) {
                return false;
            }
            if (ProjectID == 0 || Path == undefined) {
                return false;
            }
            // get other variables
            var pub = $('#RadioPublic').prop('checked');
            var byuser = $('#RadioUser').prop('checked');
            if (pub)
            {
                 $.ajax({
                    type: "POST",
                    url: "PProject.aspx/SetProjectPrivacyPublic",
                    data: '{ProjectID:"' + ProjectID + '", Files:' + JSON.stringify(Files) + ', Public: "' + pub + '", Dir:"' + Path + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $('#PrivacyModal').modal('hide');
                    },
                    error: function (msg, k, l) {
                        PError('Error:' + msg.responseText);
                    }
                });
            }
            else
            {
            if (byuser) {
                $.ajax({
                    type: "POST",
                    url: "PProject.aspx/SetProjectPrivacyByUser",
                    data: '{ProjectID:"' + ProjectID + '", Files:' + JSON.stringify(Files) + ', Users: ' + JSON.stringify(PrivacyUsers) + ', Dir:"' + Path + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $('#PrivacyModal').modal('hide');
                    },
                    error: function (msg, k, l) {
                        PError('Error:' + msg.responseText);
                    }
                });
            }
            else {
                var byrole = $('#RadioRole').prop('checked');
                GetRoles();
                if (byrole) {
                    $.ajax({
                        type: "POST",
                        url: "PProject.aspx/SetProjectPrivacyByRole",
                        data: '{ProjectID:"' + ProjectID + '", Files:' + JSON.stringify(Files) + ', Roles: ' + JSON.stringify(privacyRoles) + ', Dir:"' + Path + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            $('#PrivacyModal').modal('hide');
                        },
                        error: function (msg, i, k) {
                            PError('Error: ' + msg.responseText);
                        }
                    });
                }
            }
            }

        }

        function file() {
            window.location.href = ClickedFilePath;
        }

        function ShowFile(obj) {
            $a = $(obj);
            var path = $a.prop('href');
            var t = $a.text();
                ClickedFileName = t;
            if (path.split('.')[1] != 'htm') {
                ClickedFilePath = path;
                Discussions('File');
            }
            else {
                
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
        // #endregion

        // #region ProjectDiscussion
        function Discussions(obj) {
            if (obj == 'Project') {
                if (ProjectID != 0) {
                    $('#ProjectD').show();
                    //$('#FileD').hide();
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
        function saveDiscussion() {
            if (DisModal == 'Project') {
                if (ProjectID == 0) {
                    PError('Error with project id!');
                }
                else {
                    var title = $('#tbDisTitle').val();
                    var text = $('#tbDisText').val();
                        $.ajax({
                            type: "POST",
                            url: "PProject.aspx/SaveDiscussion",
                            data: '{ProjectID:"' + ProjectID + '", title:"' + title + '", text: "' + text + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (response) {
                                if (response.d != "") {
                                    $('#DiscussionModal').modal('hide');
                                    $('#<%=DiscussionList.ClientID %>').prepend(response.d);
                                }
                                else {
                                    $('#DiscussionModal').modal('hide');
                                    PError('Error saving the Discussion!');
                                }
                            },
                            error: function () {
                                $('#DiscussionModal').modal('hide');
                                PError('Error saving the Discussion, Try again!');
                            }
                        });
                    

                }
            }
                else {
                    if (DisModal == 'File' && ClickedFileName != '') {
                        // save file discussion
                        var title = $('#tbDisTitle').val();
                        var text = $('#tbDisText').val();
                        $.ajax({
                            type: "POST",
                            url: "PProject.aspx/SaveFileDiscussion",
                            data: '{Filename:"' + ClickedFilePath + '", title:"' + title + '", text: "' + text + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (response) {
                                if (response.d != "") {
                                    $('#DiscussionModal').modal('hide');
                                    $('#<%=FileDiscussionList.ClientID %>').prepend(response.d);
                                }
                                else {
                                    $('#DiscussionModal').modal('hide');
                                    PError('Error saving the Discussion!');
                                }
                            },
                            error: function () {
                                $('#DiscussionModal').modal('hide');
                                PError('Error saving the Discussion, Try again!');
                            }
                        });
                    }
            }
            
        }
        function openDisModal(obj) {
            if (obj == 'Project') {
                DisModal = 'Project';
            }
            else {
                if (obj == 'File') {
                    DisModal = 'File';
                }
            }
            
            $('#DiscussionModal').modal('show');
        }

        function GetDiscussions(obj) {
            if (obj == 'Project') {
                if (ProjectID != 0) {
                    $.ajax({
                        type: "POST",
                        url: "PProject.aspx/GetDiscussions",
                        data: '{ProjectID:"' + ProjectID + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response.d != "") {
                                $('#<%=DiscussionList.ClientID %>').html(response.d);
                                $('#DiscussionPreview').hide();
                            }
                            else {
                                $('#<%=DiscussionList.ClientID %>').html('');
                                $('#DiscussionPreview').hide();
                            }
                        },
                        error: function () {
                            PError('Error fetching meeting discussions, Try again!');
                        }
                    });
                }
            }
            else {
                // get file discussions
                if (ClickedFileName != '') {
                    $.ajax({
                        type: "POST",
                        url: "PProject.aspx/GetFileDiscussions",
                        data: '{Filename:"' + ClickedFilePath + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response.d != "") {
                                $('#<%=FileDiscussionList.ClientID %>').html(response.d);
                                $('#FileDiscussionPreview').hide();
                            }
                            else {
                                $('#<%=FileDiscussionList.ClientID %>').html('');
                                $('#FileDiscussionPreview').hide();
                            }
                        },
                        error: function () {
                            PError('Error fetching meeting discussions, Try again!');
                        }
                    });
                }
            }
        }
        function ShowDiscussion(obj) {
            DisID = $(obj).prop('title');
            $.ajax({
                type: "POST",
                url: "PProject.aspx/GetDiscussion",
                data: '{disId:"' + DisID + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d != "") {
                    
                        $('#DiscussionInfo').html(response.d);
                        $('#DiscussionPreview').show();
                        $('#AddComment').show();
                   
                        return false;
                    }
                    else {
                        PError('Error fetching discussion!');
                    }
                },
                error: function () {
                    PError('Error fetching discussion, Try again!');
                }
            });
        }
        function saveReply() {
            var title = $('#tbDisCommentTitle').val();
            var text = $('#tbDisComment').val();
            $.ajax({
                type: "POST",
                url: "PProject.aspx/SaveReply",
                data: '{disID:"' + DisID + '", title:"' + title + '", text:"' + text + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d != "") {
                        $('#Replies').append(response.d);
                        $('#tbDisCommentTitle').val('');
                        $('#tbDisComment').val('');
                    }
                    else {
                        PError('Error fetching meeting discussions!');
                    }
                },
                error: function () {
                    PError('Error fetching meeting discussions, Try again!');
                }
            });
        }


      
        // #endregion

        // #region FileDiscussion
        function ShowFileDiscussion(obj) {
            DisID = $(obj).prop('title');
            $.ajax({
                type: "POST",
                url: "PProject.aspx/GetDiscussion",
                data: '{disId:"' + DisID + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d !='')
                    {
                        $('#FileDiscussionInfo').html(response.d);
                        $('#FileDiscussionPreview').show();
                        $('#FileAddComment').show();
                        return false;
                        }
                        else {
                        PError('Error fetching discussion!');
                    }
                    
                },
                error: function () {
                    PError('Error fetching discussion, Try again!');
                }
            });
        }
        function saveFileReply() {
            var title = $('#FiletbDisCommentTitle').val();
            var text = $('#FiletbDisComment').val();
            $.ajax({
                type: "POST",
                url: "PProject.aspx/SaveFileReply",
                data: '{disID:"' + DisID + '", title:"' + title + '", text:"' + text + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d != "") {
                        $('#Replies').append(response.d);
                        $('#FiletbDisCommentTitle').val('');
                        $('#FiletbDisComment').val('');
                    }
                    else {
                        PError('Error fetching meeting discussions!');
                    }
                },
                error: function () {
                    PError('Error fetching meeting discussions, Try again!');
                }
            });
        }
        // #endregion

  // #region Task Assignment
        function ShowTask(obj) {
            var $btn = $(obj);
            var txt = $btn.text();
            $('#btnAction').text(txt);
            return false;
        }
        function AddTaskParti() {
            var name = $('#<%=tbAgendaTaskBy.ClientID %>').val();
            if (name == 'All') {
                $('#tdTaskNames').html('');
                var txt = ' <div class="alert alert-info" style="display: inline-block"><button type="button" class="btn-mini close " data-dismiss="alert"></button>' + name + '</div> ';
                var parent = $('#tdTaskNames');
                $(txt).hide().appendTo(parent).show('slow');
                $('#<%=tbAgendaTaskBy.ClientID %>').val('');
                return false;
            }
            else {
                var users = new Array();
                $('#tdTaskNames > div').each(function (index, div) {
                    $d = $(div);
                    users.push($d.text());
                });
                if ($.inArray(name, users) > -1) {
                    $('#<%=tbAgendaTaskBy.ClientID %>').val('');
                    return false;
                }

                var txt = ' <div class="alert alert-info" style="display: inline-block"><button type="button" class="btn-mini close " data-dismiss="alert"></button>' + name + '</div> ';
                var parent = $('#tdTaskNames');
                $(txt).hide().appendTo(parent).show('slow');
                $('#<%=tbAgendaTaskBy.ClientID %>').val('');
            }
        }
        function resetAddTask() {
            $('#btnAction').text('Choose Action');
            $('#tbTaskDesc').val('');
            $('#btnAgendaDueDate').val('');
            var users = new Array();
            $('#tdTaskNames').html('');
        }
        function SaveFileTask() {
            if (ProjectID != 0) {
                var title = $('#btnAction').text();
                if (title == 'Choose Action') {
                    return false;
                }
                var desc = $('#tbTaskDesc').val();
                var date = $('#btnAgendaDueDate').val();
                var users = new Array();
                $('#tdTaskNames > div').each(function (index, div) {
                    $d = $(div);
                    users.push($d.text());
                });
                if (users.length == 0) {
                    return false;
                }
                if (_EditTask == 0) {
                    // call server /save
                    $.ajax({
                        type: "POST",
                        url: "PProject.aspx/SaveFileTask",
                        data: '{ProjectID:"' + ProjectID+ '", Title:"' + title + '", Desc:"' + desc + '", DueDate:"' + date + '", usernames:' + JSON.stringify(users) + ', FilePath:"'+ClickedFilePath+'"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            var txt = response.d;
                            if ($('#divAgendaTasks').html() == '<br> No tasks') {
                                $('#divAgendaTasks').html('');
                            }
                            var parent = $('#divAgendaTasks');
                            $(txt).hide().appendTo(parent).show('slow');
                            resetAddTask();
                        },
                        error: function () {
                            PError('Error saving task, Try again!');
                        }
                    });
                }
                else {
                    // update
                    $.ajax({
                        type: "POST",
                        url: "PProject.aspx/UpdateFileTask",
                        data: '{TaskID: "' + _EditTask + '", ProjectID:"' + ProjectID + '", Title:"' + title + '", Desc:"' + desc + '", DueDate:"' + date + '", usernames:' + JSON.stringify(users) + '}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            var txt = response.d;
                            var parent = $('#div' + _EditTask);
                            parent.replaceWith(txt);
                            //parent.html('');
                            //$(txt).hide().appendTo(parent).show('slow');

                            NewTaskEntry();
                        },
                        error: function () {
                            PError('Error saving agenda task, Try again!');
                        }
                    });
                }
            }
        }

        function GetFileTasks() {
        if (ProjectID == 0)
        {
        return false;
        }
                $.cookie('FilePath', ClickedFilePath);
                $.ajax({
                    type: "POST",
                    url: "PProject.aspx/GetProjectFileTasks",
                    data: '{FileDirectory:"' + ClickedFilePath + '", ProjectID:"'+ProjectID+'"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var result = response.d;
                        if (result == "") {
                            $('#divAgendaTasks').html('<br/> No tasks');
                        }
                        else {
                            $('#divAgendaTasks').html('');
                            var parent = $('#divAgendaTasks');
                            $(result).hide().appendTo(parent).show('slow');

                        }
                    },
                    error: function () {
                        PError('Error fetching tasks for agenda, Try again!');
                    }
                });
                resetAddTask();
            
        } // done
        function AgendaTaskEdit(id) {
            if (id != 0) {
                EditAgenda = id;
            }
            $('#AgendaTab a[href="#AgendaTask"]').tab('show');
            GetFileTasks();
        }
        function EditTask(obj) {
            var id = $(obj).prop("id").split('.')[1];
            _EditTask = id;
            $.ajax({
                type: "POST",
                url: "PProject.aspx/GetFileTask",
                data: '{TaskID:"' + id + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var result = response.d;
                    $('#btnAction').text(result.Title);
                    $('#tbTaskDesc').val(result.Desc);
                    $('#btnAgendaDueDate').val(formatDate(result.DueDate));

                    var users = result.Usernames;
                    $('#tdTaskNames').html('');
                    for (var u = 0; u < users.length; u++) {
                        var txt = ' <div class="alert alert-info" style="display: inline-block"><button type="button" class="btn-mini close " data-dismiss="alert"></button>' + users[u] + '</div> ';
                        var parent = $('#tdTaskNames');
                        $(txt).hide().appendTo(parent).show('slow');
                    }
                    $('#btnAction').focus();
                    $('#btnTaskNewEntry').show('slow');
                },
                error: function () {
                    PError('Error fetching task information, Try again!');
                }
            });

            return false;
            // load data into panel

        }
        function DeleteTaskConfirm(obj) {
            var id = $(obj).prop("id").split('.')[1];
            ConfirmTask('Delete Task', 'Are you sure you want to delete the task?', id);
            return false;
        }
        function DeleteFileTask(id) {
            $.ajax({
                type: "POST",
                url: "PProject.aspx/DeleteFileTask",
                data: '{TaskID:"' + id + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var result = response.d;
                    if (result == "true") {
                        GetFileTasks();
                    }
                    else {
                        PError('Error deleting the task');
                    }
                },
                error: function () {
                    PError('Error deleting the task, Try again!');
                }
            });
            return false;
        }
        function NewTaskEntry() {
            resetAddTask();
            _EditTask = 0;
        }

        function EditUserTaskResponse(obj) {
            var id = $(obj).prop("id").split('.')[1];
            var PDiv = $(obj).parent().parent();
            $.ajax({
                type: "POST",
                url: "PProject.aspx/GetUserTaskResponse",
                data: '{TaskID:"' + id + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var result = response.d;
                    if (result != "") {
                        $(obj).parent().prev('hr').remove();
                        $(obj).parent().remove();
                      
                        //PDiv.find('hr:first-child').remove();
                        //PDiv.find('div:first-child').remove();
                        $(result).hide().appendTo(PDiv).show('slow');
                    }
                    else {
                        PError('Error fetching information');
                    }
                },
                error: function () {
                    PError('Error fetching information, Try again!');
                }
            });

            return false;
        }

        function btnTaskResponseClick(obj) {
            var $btn = $(obj);
            var txt = $btn.text();
            var id = $btn.prop("id").split('.')[1];
            $('#btnTaskResponse' + id).text(txt);

            return false;
        }

        function SaveTaskResponse(obj) {
            var id = $(obj).prop("id").split('.')[1];
            // get reponse and desc
            var text = $('#textarea' + id).val();
            var res = $('#btnTaskResponse' + id).text();

            $.ajax({
                type: "POST",
                url: "PProject.aspx/SaveTaskResponse",
                data: '{TaskID:"' + id + '", Response:"' + res + '", Desc:"' + text + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var result = response.d;
                    if (result != "") {
                        UpddateUIAfterSave(id, result);
                    }
                    else {
                        PError('Error saving the response');
                    }
                },
                error: function () {
                    PError('Error saving the response, Try again!');
                }
            });

        }

        function UpddateUIAfterSave(id, result) {
            // add the response - append to  th tasks
            //var parent = $('#div' + id);
            var parent = $('#divTool' + id).parent();
            $(result).hide().appendTo(parent).show('slow');
            // hide the tool
            $('#divTool' + id).prev('hr').remove();
          
            $('#divTool' + id).hide('slow').remove();
            
        }
        // #endregion

        // #region Edit
        function EditFile() {
            // get the content inside frame
            var fileTypes = new Array("DOC", "DOCX", "XLS" , "XLSX", "PPT", "PPTX", "TXT","XML", "CSS","HTML", "HTM");
            var ex = ClickedFileName.split('.')[1];
            ex = ex.toUpperCase();
            var contain = $.inArray(ex,fileTypes);
            if (contain != -1)
            {
            var text = window.frames['iframe01'].document.body.innerHTML;
            CKEDITOR.instances.<%= CKEditor1.ClientID%>.setData(text);
           }
           else
           {
           PError('You can not edit this file type');
           }
            //alert($('#iframe01').contents().find("#myContent").html());
        }
        function load() {
            CKEDITOR.instances.<%= CKEditor1.ClientID%>.setData('<p>This is the editor data.</p>');
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
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <body>
                <h2>
                    Projects</h2>
                    
              
                <%HandleAction();%>
                <form action="<%=ScriptName%>" method="post" enctype="multipart/form-data" data-ajax="false">
                <font size="3">
                <input type="hidden" name="<%=ActionTag%>" />
                <%  WriteTable(); %>
                <br />
                <br />
                </font>
                <input type="hidden" runat="server" id="hiddenUsers" />
                <div id="iframDiv" hidden="hidden" style="overflow-x: hidden; overflow: auto; background-color: #ECECEE;
                    padding: 10px; border: 2px solid #CCCCCC">
                    <div class="navbar-inverse">
                        <ul id="myTab" class="nav nav-tabs " style="background-color: Black; font-size:small">
                            <li class="active"><a href="#View" data-toggle="tab">View</a></li>
                            <li><a href="#Edit" data-toggle="tab" onclick="EditFile()">Edit</a></li>
                            <li id="FileD" hidden="hidden"><a href="#FileDiscussions" data-toggle="tab" onclick="GetDiscussions('File')">
                                File Discussions</a></li>
                            <li id="ProjectD" hidden="hidden"><a href="#ProjectDiscussions" data-toggle="tab">Project
                                Discussions</a></li>
                            <li><a href="#FileTask" data-toggle="tab" onclick="GetFileTasks()">Tasks Assignment</a></li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane active" id="View" style="height: 500px">
                                <input type="button" class="btn pull-right" value="Download" onclick="file()" />
                                <br />
                                <iframe runat="server" scrolling="no" frameborder="0" id='iframe01' name='iframe01'
                                    style="background-color: White; height: 100%" width="100%" onload="contentFrame_onLoadServer" />
                            </div>
                            <div class="tab-pane" id="ProjectDiscussions">
                                Discussions
                                <input class="btn-mini btn-info pull-right" type="button" onclick="openDisModal('Project')"
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
                                <CKEditor:CKEditorControl ID="CKEditor1" BasePath="/ckeditor/" runat="server" Skin="office2003"></CKEditor:CKEditorControl>
                                <input type="button" onclick="load()" />
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
                            <div class="tab-pane" id="FileTask">
                                <h4>
                                    Tasks Assignment</h4>
                                <div id="divAgendaTasks" style="font-size:small">
                                    <div class="taskBox">
                                        <table>
                                            <tr>
                                                <td style="width: 80px">
                                                    Action:
                                                </td>
                                                <td>
                                                    l;kjfsdkljhglskjghslkdjfg
                                                </td>
                                                <td style="width: 100px">
                                                    Describtion
                                                </td>
                                                <td>
                                                    asdfaskdgbalgh
                                                </td>
                                                <td style="width: 100px">
                                                    Due Date
                                                </td>
                                                <td style="border: 1px solid red; background-color: #FF4D4D; color: White">
                                                    asdgsdfgdgdgdgdgdf
                                                </td>
                                            </tr>
                                        </table>
                                        <hr />
                                        <div class="row-fluid">
                                            <div>
                                                <div>
                                                    <b>Marwa Alhazmi</b>
                                                    <div class="pull-right">
                                                        <b>Completed</b> 12/12/2013 10:10:20 PM
                                                    </div>
                                                </div>
                                                <br />
                                                <div>
                                                    The ljalkjf; kdjfakdj ;akjdf;ak skjhdsjhg lajdljg gfhgds hgfd gf gf dfds fd gfd
                                                    sgfd gfd sgfd gfd gfd gfd sgfd sfd sThe ljalkjf; kdjfakdj ;akjdf;ak skjhdsjhg lajdljg
                                                    gfhgds hgfd gf gf dfds fd gfd sgfd gfd sgfd gfd gfd gfd sgfd sfd sThe ljalkjf; kdjfakdj
                                                    ;akjdf;ak skjhdsjhg lajdljg gfhgds hgfd gf gf dfds fd gfd sgfd gfd sgfd gfd gfd
                                                    gfd sgfd sfd sThe ljalkjf; kdjfakdj ;akjdf;ak skjhdsjhg lajdljg gfhgds hgfd gf gf
                                                    dfds fd gfd sgfd gfd sgfd gfd gfd gfd sgfd sfd s
                                                </div>
                                                <a href="#" onclick="return EditUserTaskResponse(this)" class="pull-right" style="font-size: smaller"
                                                    id="a.5">Edit</a>
                                            </div>
                                            <hr />
                                            <div class="span10">
                                                <div class="btn-group">
                                                    <button class="btn" id="Button1" style="width: 150px" onclick="return false">
                                                        Choose Action</button>
                                                    <button data-toggle="dropdown" class="btn dropdown-toggle">
                                                        <span class="caret"></span>
                                                    </button>
                                                    <ul class="dropdown-menu" role="menu" aria-labelledby="dLabel">
                                                        <li><a href="#" onclick="return ShowTask(this)">Complete</a></li><li><a href="#"
                                                            onclick="return ShowTask(this)">Comment</a></li><li><a href="#" onclick="return ShowTask(this)">
                                                                Decline Task</a></li>
                                                </div>
                                                <br />
                                                <textarea id="textarea1" class="input-block-level" placeholder="Insert a comment"
                                                    cols="" rows="" style="width: 100%; height: 90px">
                                                                </textarea>
                                                <div class="pull-right">
                                                    <input type="button" value="Send" class="btn-primary" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <hr />
                                <div class="row-fluid" id="divAddAgendaTask" runat="server" visible="false">
                                    <fieldset>
                                        <legend>New Task</legend>
                                        <div class="pull-right">
                                            <input type="button" class="btn-primary" value="Save" onclick="SaveFileTask()" /><input
                                                type="button" id="btnTaskNewEntry" value="New Entry" onclick="NewTaskEntry()"
                                                hidden="hidden" /></div>
                                        <div class="span5">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td style="width: 150px">
                                                        Action Required
                                                    </td>
                                                    <td>
                                                        <div class="btn-group">
                                                            <button class="btn" id="btnAction" style="width: 150px" onclick="return false">
                                                                Choose Action</button>
                                                            <button data-toggle="dropdown" class="btn dropdown-toggle">
                                                                <span class="caret"></span>
                                                            </button>
                                                            <ul class="dropdown-menu" role="menu" aria-labelledby="dLabel">
                                                                <li><a href="#" onclick="return ShowTask(this)">To Review</a></li><li><a href="#"
                                                                    onclick="return ShowTask(this)">To Approve</a></li><li><a href="#" onclick="return ShowTask(this)">
                                                                        To Update</a></li><li class="divider"></li>
                                                                <li><a href="#" onclick="return ShowTask(this)">Other</a></li></ul>
                                                        </div>
                                                        <br />
                                                    </td>
                                                </tr>
                                                <tr style="width: 15%; height: 50px">
                                                    <td>
                                                        Due Date
                                                    </td>
                                                    <td>
                                                        <input id="btnAgendaDueDate" style="width: 185px" type="text" class="datepicker" />
                                                    </td>
                                                </tr>
                                                <tr style="width: 15%; height: 50px">
                                                    <td style="vertical-align: top">
                                                        By
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="tbAgendaTaskBy" Width="185px" runat="server"></asp:TextBox><ajax:AutoCompleteExtender
                                                            ID="AutoCompleteExtender1" runat="server" TargetControlID="tbAgendaTaskBy" OnClientItemSelected="AddTaskParti"
                                                            UseContextKey="True" MinimumPrefixLength="1" ServiceMethod="GetCompletionList"
                                                            EnableCaching="false">
                                                        </ajax:AutoCompleteExtender>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div class="span5">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <table style="width: 100%">
                                                            <tbody>
                                                                <tr>
                                                                    <td style="width: 150px; font-size: small">
                                                                        Describtion
                                                                    </td>
                                                                    <td>
                                                                        <textarea id="tbTaskDesc" cols="20" rows="2"></textarea>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <tr>
                                                            <td id="tdTaskNames">
                                                            </td>
                                                        </tr>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </fieldset>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <br />
                <div id="NewProjectModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
                    aria-hidden="true">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                        </button>
                        <h3 id="H1">
                            New Project</h3>
                    </div>
                    <div class="modal-body">
                        <div id="modalProjectName">
                            <h4>
                                Project Name</h4>
                            <br />
                            <input type="text" name="tbProjectName" id="tbProjectNameID" />
                            <hr />
                        </div>
                        <h4>
                            Project Users</h4>
                        <br />
                        <div class="row-fluid">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 50%">
                                        User
                                    </td>
                                    <td>
                                        Role
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:ListBox ID="lbUsers" runat="server" AppendDataBoundItems="true">
                                            <asp:ListItem Value="All">All</asp:ListItem>
                                        </asp:ListBox>
                                    </td>
                                    <td>
                                        <asp:ListBox ID="lbRole" runat="server"></asp:ListBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                        <a href="#" class="win-command pull-right" onclick="AddProjectUser()"><span class=" win-command icon-plus-5">
                                        </span></a>
                                    </td>
                                </tr>
                            </table>
                            <hr />
                            <table id="tblUsers" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>
                                            User
                                        </th>
                                        <th>
                                            Role
                                        </th>
                                        <th>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button class="btn" data-dismiss="modal">
                            Close</button>
                        <input type="button" id="tbSave" class="btn btn-primary" value="Save" onclick="newProject()" />
                    </div>
                </div>
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
                <div id="PrivacyModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
                    aria-hidden="true">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                        </button>
                        <h3>
                            Privacy Settings</h3>
                    </div>
                    <div class="modal-body">
                        <h4>
                            <b>Files</b></h4>
                        <div class="row-fluid" style="font-size: small">
                            <div id="divPrivacyFiles1" style="padding-left: 20px">
                            </div>
                        </div>
                        <hr />
                        <table>
                            <tr>
                                <td style="width: 150px">
                                    <h4>
                                        <b>Privacy</b></h4>
                                </td>
                                <td style="width: 100px">
                                    <label class="radio inline">
                                        <input type="radio" name="optionsRadios" id="RadioPublic" value="option1" checked
                                            onclick="ShowPrivacyBy('Public')">
                                        <span class="metro-radio">Public</span>
                                    </label>
                                </td>
                                <td>
                                    <label class="radio inline">
                                        <input type="radio" name="optionsRadios" id="RadioPrivate" value="option1" onclick="ShowPrivacyBy('Private')">
                                        <span class="metro-radio">Private</span>
                                    </label>
                                </td>
                            </tr>
                        </table>
                        <div id="divPrivacyPrivate" hidden="hidden">
                            <table>
                                <tr>
                                    <td style="width: 150px">
                                        <h4>
                                            Allow Access</h4>
                                    </td>
                                    <td style="width: 100px">
                                        <label class="radio inline">
                                            <input type="radio" name="optionsRadios1" id="RadioRole" value="option1" onclick="ShowPrivacyUsers('Role')">
                                            <span class="metro-radio">By Role</span>
                                        </label>
                                    </td>
                                    <td>
                                        <label class="radio inline">
                                            <input type="radio" name="optionsRadios1" id="RadioUser" value="option1" onclick="ShowPrivacyUsers('User')">
                                            <span class="metro-radio">By User</span>
                                        </label>
                                    </td>
                                </tr>
                            </table>
                            <hr />
                        </div>
                        <div id="divPrivacyUsers" class="row-fluid" hidden="hidden">
                        </div>
                        <div id="divPrivacyRole" hidden="hidden">
                            <label class="checkbox inline">
                                <input type="checkbox" value="admin" onclick="chkRoles(this)" checked="checked"><span class="metro-checkbox">Admin</span></label>
                            <label class="checkbox inline">
                                <input type="checkbox" value="moderator" onclick="chkRoles(this)" checked="checked"><span class="metro-checkbox">Moderator</span>
                            </label>
                            <label class="checkbox inline">
                                <input type="checkbox" value="editor" onclick="chkRoles(this)" checked="checked"><span class="metro-checkbox">Editor</span></label>
                            <label class="checkbox inline">
                                <input type="checkbox" value="commentor" onclick="chkRoles(this)" checked="checked"><span class="metro-checkbox">Commentor</span></label>
                            <label class="checkbox inline">
                                <input type="checkbox" value="user" onclick="chkRoles(this)" checked="checked"><span class="metro-checkbox">User</span></label></div>
                    </div>
                    <div class="modal-footer">
                        <button class="btn" data-dismiss="modal">
                            Close</button>
                        <input type="button" id="btnPrivacySave" class="btn btn-primary" value="Save" onclick="SaveProjectPrivacy()" />
                    </div>
                </div>
                <div id="DiscussionModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
                    aria-hidden="true">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                        </button>
                        <h3 id="H2">
                            New discussion</h3>
                    </div>
                    <div class="modal-body">
                        <div style="margin-left: 15px">
                            Title:
                            <input type="text" id="tbDisTitle" style="width: 450px" />
                        </div>
                        <textarea id="tbDisText" cols="4" rows="2" style="width: 490px; height: 200px; margin-left: 15px"></textarea>
                        <div id="FileDiv" hidden="hidden" style="margin-left: 15px">
                            <label class="checkbox">
                                <input id="cbFile" type="checkbox"><span id="cbText" class="metro-checkbox"></span>
                            </label>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button class="btn" data-dismiss="modal">
                            Close</button>
                        <input type="button" id="btnDisSave" class="btn btn-primary" onclick="saveDiscussion()"
                            value="Save" />
                    </div>
                </div>
                <div aria-hidden="true" aria-labelledby="myModalLabel" role="dialog" tabindex="-1"
                    class="modal message hide fade" id="PartiModal" style="display: none;">
                    <div class="modal-header">
                        <button aria-hidden="true" data-dismiss="modal" class="close" type="button">
                        </button>
                        <br />
                        <h4 id="headerConfirm">
                            Disinviting a participant</h4>
                    </div>
                    <div class="modal-body">
                        <p id="txtConfirm">
                            Are you sure you want to disinvite <span id="partiModaltxt"></span>from the meeting?
                        </p>
                    </div>
                    <div class="modal-footer">
                        <button id="btnConfirm" data-dismiss="modal" class="btn btn-primary">
                            Yes</button>
                        <button data-dismiss="modal" class="btn">
                            Cancel</button>
                    </div>
                </div>
                </form>
            </body>
        </ContentTemplate>
    </asp:UpdatePanel>
    </asp:Content>