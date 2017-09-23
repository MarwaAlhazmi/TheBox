<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="PMeetingSetUp.aspx.cs" Inherits="TheBox.PMeetingSetUp" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="https://apis.google.com/js/plusone.js"></script>
    <script src="content/js/datepair.js" type="text/javascript"></script>
    <script type="text/javascript" src="content/js/jquery.cookie.js"></script>
    <script>
        // #region General 
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
        var MeetingID = 0;
        var EditAgenda = 0;
        var DisModal = true;
        var DisID = 0;
        var _EditAgendaTask = 0;
        function Event(username, role, note) {
            this.UserName = username;
            this.Role = role;
            this.Note = note;
        }
        function setMeetingID(id, role) {
            MeetingID = id;
            AgendaUI(role);
        }
        function Privacy(username, cansee) {
            this.UserName = username;
            this.CanSee = cansee;
        }
        function Success(msg) {
            $('#pSuccess').text(msg);
            $('#MyModalSuccess').modal('show').delay('1000');
            $('#MyModalSuccess').fadeOut(1000, function () {
                $('#MyModalSuccess').modal('hide');
            });
        }
        function ConfirmParti(hd, msg) {
            $('#btnConfirm').attr('onclick', '');
            $('#btnConfirm').unbind('click');
            $('#btnConfirm').click(function () {
                DeleteParticipant();
            });
            $('#PartiModal').modal('show');
            $('#txtConfirm').html(msg);
            $('#headerConfirm').text(hd);
        }

        function ConfirmTask(hd, msg, id) {
            $('#btnConfirm').attr('onclick', '');
            $('#btnConfirm').unbind('click');
            $('#btnConfirm').click(function () {
                DeleteAgendaTask(id);
            });
            $('#PartiModal').modal('show');
            $('#txtConfirm').html(msg);
            $('#headerConfirm').text(hd);
        }

        function PError(msg) {
            $('#pError').text(msg);
            $('#MyModalError').modal('show');
        }
        function invite() {
            var id = parseInt($('#<%=parTable.ClientID %> tr:last td:first').text());
            if (isNaN(id)) {
                alert('error');
                return;
            }
        }

        $(document).ready(function () {

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
            if (urlParams["tab"] == "files") { showResources(); }
            if (urlParams["tab"] == "discussion") { showDiscussion(); }
            if (urlParams["tab"] == "participants") { showParti(); }
            if (urlParams["tab"] == "agenda") { showAgenda(); }

            if (urlParams["id"] != '') {
                var id = urlParams["id"];
                MeetingID = id;
                $.ajax({
                    type: "POST",
                    url: "PMeetingSetUp.aspx/CheckRole",
                    data: '{meetingID:"' + id + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        AgendaUI(response.d);
                        // activate 
                    },
                    error: function () {
                    }
                });

            }
        });

        // #endregion 
        // #region MeetingInfo

        function GetAddedUserNames() {
            var UserList = new Array();
            $('#<%=parTable.ClientID %> tr').each(function (index, tr) {
                var $row = $(tr);

                var txt = $row.find('td:nth-child(2)').text();
                if (txt != '') {
                    UserList.push(txt);
                }

            });
            return UserList;
            //for (var i = 0; i < UserList.length; i++) {
            //    alert(UserList[i]);
            //}
        }
        function Delete(obj) {
            var $tr = $(obj).closest('tr');
            // check role
            var role = $tr.find('td:nth-child(3)').text();
            if (role == 'Creator') {
                alert('You Cannot Delete Meeting creator');
                return false;
            }

            $tr.fadeOut(400, function () {
                $tr.remove();
            });

            return false;
        }
        function getMeetingInfo(id) {
        }
        function cc() {
            if ($('#chk').is(':checked')) {
                $('#lblText').text($('#chk').val());
            }
            else {
                $('#lblText').html("false");
            }
        }

        function showParti() {
            $('#myTab a[href="#Participants"]').tab('show');
            getParticipants();
        }
        function showResources() {
            $('#myTab a[href="#Resources"]').tab('show');
        }
        function showAgenda() {
            $('#myTab a[href="#Agenda"]').tab('show');
        }
        function showInfo() {
            $('#myTab a[href="#Info"]').tab('show');
        }

        function showDiscussion() {
            $('#myTab a[href="#Discussion"]').tab('show');
        }


        function checkAvl() {


            var date = $('#<%=tbSDate.ClientID%>').val();
            var stime = date + " " + $('#<%=tbSTime.ClientID%>').val();
            var etime = date + " " + $('#<%=tbETime.ClientID%>').val();
            if (loc != 0) {
                $.ajax({
                    type: "POST",
                    url: "PMeetingSetUp.aspx/checkAvailability",
                    data: '{"date:"' + date + '", stime:"' + stime + '", etime:"' + etime + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $('#progressDiv').hide();
                        if (response.d == "true") {
                            $('#result').css('color', 'green');
                            $('#result').html("Available");
                            $('#avlRooms').hide();
                        }
                        else {
                            $('#result').css('color', 'red');
                            $('#result').html("Meeting Room is Unavailable");
                            $('#avlRooms').show();
                        }
                    }
                });
            }
        }
        function AllDay() {
            if ($('#cbAllDay').is(':checked')) {

                $('#<%=tbETime.ClientID %>').prop("disabled", true);
                $('#<%=tbETime.ClientID %>').val('');
            }
            else {

                $('#<%=tbETime.ClientID %>').prop("disabled", false);
            }
        }
        function SaveInfo() {
            var id = $('#<%=tbID.ClientID %>').val();
            var title = $('#<%=tbtitle.ClientID %>').val();
            var sdate = $('#<%=tbSDate.ClientID %>').val();
            var edate = $('#<%=tbEDate.ClientID %>').val();
            var stime = $('#<%=tbSTime.ClientID %>').val();
            var etime = $('#<%=tbETime.ClientID %>').val();
            var desc = $('#<%=tbDesc.ClientID %> ').val();
            var type = $('#<%=ddlMType.ClientID %> option:selected').val();
            var allday = $('#cbAllDay').is(':checked');
            if (allday) {
                etime = '00:00:00';
            }
            if (title == '' || sdate == '' || edate == '' || stime == '' || etime == '') {
                return false;
            }
            // check to update or save a new entry
            if (id == '') {
                $('ProgressInfo').show('slow');
                // save new entry
                $.ajax({
                    type: "POST",
                    url: "PCalender.aspx/SaveMeet",
                    data: '{sdate:"' + sdate + '", edate:"' + edate + '", stime:"' + stime + '", etime:"' + etime + '", etitle:"' + title + '", desc:"' + desc + '", mtype:"' + type + '", aday:"' + allday + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d != 0) {
                            $('#<%=tbID.ClientID%>').val(response.d);
                            MeetingID = parseInt(response.d);
                            Success('Saved!');
                            return false;
                        }
                        else {
                            alert('error: new insert');
                            $('#ProgressInfo').hide('slow');
                        }

                    },
                    error: function (hx, status, ll) {
                        alert('error insert ' + status);
                        $('#ProgressInfo').hide('slow');
                    }
                });
            }
            else {
                // update the existing
                $('#ProgressInfo').show('slow');
                $.ajax({
                    type: "POST",
                    url: "PCalender.aspx/UpdateMeeting",
                    data: '{id:"' + id + '", sdate:"' + sdate + '", edate:"' + edate + '", stime:"' + stime + '", etime:"' + etime + '", etitle:"' + title + '", desc:"' + desc + '", mtype:"' + type + '", aday:"' + allday + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d != 0) {
                            MeetingID = parseInt(id);
                            Success('Information Updated');
                            $('#ProgressInfo').hide('slow');
                            return false;
                        }
                        else {
                            alert('error: Update');
                            $('#ProgressInfo').hide('slow');
                        }
                    },
                    error: function (hx, status, ll) {
                        alert('error update ' + status);
                        $('#ProgressInfo').hide('slow');
                    }
                });
            }
        }
        // #endregion

        // Participants Functions -----------------------------------------------------------------
        // #region Participants
        function AddParticipant() {
            if (MeetingID == 0) {
                PError('You need to save Meeting information first!');
                return false;
            }
            var user = $('#<%=lbPart.ClientID %> option:selected').val();
            var role = $('#<%=lbRole.ClientID %> option:selected').val();
            var text = $('#tbNote').val();
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

            var id = parseInt($('#<%=parTable.ClientID %> tr:last td:first').text()) + 1;
            if (isNaN(id)) {
                id = 1;

            }
            var tag = "<tr><td>" + id + "</td>" + "<td>" + user + "</td>" + "<td>" + role + "</td>" + "<td>None</td>" + "<td>" + text + "</td><td><a href='#' onclick='return Delete(this)'><span class='icon-x'></span></a></td></tr>";
            $('#<%=parTable.ClientID %> > tbody:last').append(tag);
        }

        function getParticipants() {

            if (MeetingID != 0 && !isNaN(MeetingID)) {
                $.ajax({
                    type: "POST",
                    url: "PMeetingSetUp.aspx/GetParticipants",
                    data: '{id:"' + MeetingID + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $('#<%=parTable.ClientID %>').find("tr:gt(0)").remove();
                        var list = response.d;
                        $(list).each(function (index, parti) {
                            var user = parti.UserName;
                            var role = parti.Role;
                            var note = parti.Note;
                            var response = parti.Response;
                            var id = parseInt($('#<%=parTable.ClientID %> tr:last td:first').text()) + 1;
                            if (isNaN(id)) { id = 1; }
                            var disinvite = "";
                            var tag = '<tr><td>' + id + '</td>' + '<td>' + user + '</td>' + '<td>' + role + '</td>' + '<td>' + response + '</td>' + '<td>' + note + '</td><td></td></tr>';
                            if (role != 'Creator') {
                                tag = '<tr><td>' + id + '</td>' + '<td>' + user + '</td>' + '<td>' + role + '</td>' + '<td>' + response + '</td>' + '<td>' + note + '</td><td><input value="Disinvite" class="btn-mini btn-info" type="button" onclick="DeleteParticipantModal(this)"/></td></tr>';
                            }
                            $('#<%=parTable.ClientID %> > tbody:last').append(tag);

                        });
                    }
                });
            }
        }
        function DeleteParticipantModal(row) {
            $('#PartiModal').modal('show');
            $tr = $(row).closest('tr');
            var username = $tr.find('td:nth-child(2)').text();
            var text = 'Are you sure you want to disinvite <span id="partiModaltxt">' + username + '</span> from the meeting?';
            ConfirmParti('Disinvite Participant', text);
        }
        function DeleteParticipant() {
            var username = $('#partiModaltxt').text();
            $.ajax({
                type: "POST",
                url: "PMeetingSetUp.aspx/DisinviteParticipant",
                data: '{meetingid:"' + MeetingID + '", username:"' + username + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d == 'true') {
                        getParticipants();
                    }
                },
                error: function () {
                    $('#PartiModal').modal('hide');
                    PError('Error disinviting participants, Try again!');
                }
            });
        }

        function updateParticipantUI() {
            $('#<%=parTable.ClientID %> tr').each(function (index, tr) {
                var $row = $(tr);
                var txt = $row.find('td:nth-child(4)').text();
                if (txt == 'None') {
                    $row.find('td:nth-child(4)').text('Pending');
                    $row.find('td:nth-child(6)').html('<input value="Disinvite" class="btn-mini btn-info" type="button" onclick="DeleteParticipantModal(this)"/>');
                }
            });
        }


        function SaveParticipants() {
            if (MeetingID == 0) {
                PError('You need to save Meeting information first!');
                return false;
            }
            var UserList = new Array();
            $('#<%=parTable.ClientID %> tr').each(function (index, tr) {
                var $row = $(tr);
                var txt = $row.find('td:nth-child(4)').text();
                if (txt == 'None') {
                    // get all data and save
                    var user = $row.find('td:nth-child(2)').text();
                    var role = $row.find('td:nth-child(3)').text();
                    var note = $row.find('td:nth-child(5)').text();
                    var evn = new Event(user, role, note);
                    UserList.push(evn);
                }
            });
            if (UserList.length > 0) {
                // save to db
                $('#ProgressParti').show('slow');
                $.ajax({
                    type: "POST",
                    url: "PMeetingSetUp.aspx/SaveParticipants",
                    data: '{obj:' + JSON.stringify(UserList) + ', meetingID:"' + MeetingID + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d == 'True') {
                            updateParticipantUI();
                            $('#ProgressParti').hide('slow');
                            Success('Participants added successfully');
                            return false;
                        }
                        else {
                            PError('Error Saving Participants, Try again!');
                        }
                    },
                    error: function () {
                        PError('Error Saving Participants, Try again!');
                    }
                });
            }
        }
        // #endregion

        // Resources Functions --------------------------------------------------------------------
        // #region Resources
        function saveResource() {
            var name = $('#tbResourceName').val();
            var des = $('#tbResourceDesc').val();
            $.ajax({
                type: "POST",
                url: "PMeetingSetUp.aspx/insertResource",
                data: '{name:"' + name + '", desc:"' + des + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d != 0) {
                        var id = response.d;
                        var text = '<label class="checkbox inline"><input type="checkbox" id="cb' + id + '" value="' + id + '"><span class="metro-checkbox">' + name + '</span></label></br>';
                        $('#<%=divItems.ClientID %>').append(text);
                        $('#ResourceModal').modal('hide');
                    }
                    else {
                        $('#ResourceModal').modal('hide');
                        PError('Error Saving Resource, Try again!');
                    }
                },
                error: function () {
                    $('#ResourceModal').modal('hide');
                    PError('Error Saving Resource, Try again!');
                }
            });
        }
        function UpdateResources() {
            if (MeetingID == 0) {
                PError('You need to save meeting information first');
                return false;
            }
            else {

                var selected = new Array();
                $('#<%=divItems.ClientID %> input:checked').each(function () {
                    selected.push($(this).attr('value'));
                });
                $.ajax({
                    type: "POST",
                    url: "PMeetingSetUp.aspx/UpdateResources",
                    data: '{ids: ' + JSON.stringify(selected) + ', MeetingID:"' + MeetingID + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d != 'false') {
                            Success('Resources updated successfully');
                        }
                    },
                    error: function () {
                        PError('Error Fetching Resources, Try again!');
                    }
                });
                return false;

            }
        }
        function GetResources() {
            $.ajax({
                type: "POST",
                url: "PMeetingSetUp.aspx/GetResources",
                data: '{MeetingID:"' + MeetingID + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d != null) {
                        var list = response.d;
                        for (var t = 0; t < list.length; t++) {
                            var str = '#cb' + list[t].RID;
                            $(str).prop('checked', true);
                        }
                    }
                },
                error: function () {
                    PError('Error Fetching Resources, Try again!');
                }
            });
        }
        function GetLocation() {
            $.ajax({
                type: "POST",
                url: "PMeetingSetUp.aspx/GetLocation",
                data: '{MeetingID:"' + MeetingID + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d != "") {
                        $('#MVenueTD').text(response.d);
                    }
                    else {
                        $('#MVenueTD').text('Not Set yet');
                    }
                },
                error: function () {
                    PError('Error fetching location information, Try again!');
                }
            });
        }
        function FetchResources() {
            GetResources();
            GetLocation();
            // get role
            $.ajax({
                type: "POST",
                url: "PMeetingSetUp.aspx/CheckResource",
                data: '{}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d == true) {
                        $('#btnResourceSetting').show();
                    }
                    else {
                        $('#btnResourceSetting').hide();
                    }
                }
            });

        }
        function UpdateLocation() {
            if (MeetingID == 0) {
                PError('You need to save meeting information first!');
            }
            else {
                var loc = $('#<%=lbLocation.ClientID%> option:selected').val();
                if (loc == '-1') {
                    $('#LocationModal').modal('hide');
                    return false;
                }
                $.ajax({
                    type: "POST",
                    url: "PMeetingSetUp.aspx/UpdateLocation",
                    data: '{MeetingID:"' + MeetingID + '", LocationID:"' + loc + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d == "true") {
                            $('#MVenueTD').text($('#<%=lbLocation.ClientID%> option:selected').text());
                            $('#LocationModal').modal('hide');
                        }
                        else {
                            $('#LocationModal').modal('hide');
                            $('#MVenueTD').text('Not Set yet');
                        }
                    },
                    error: function () {
                        PError('Error updating location information, Try again!');
                    }
                });
            }


        }
        function GetAvailableLocation() {
            if (MeetingID == 0) {
                PError('You need to save meeting information first');
                return false;
            }
            else {
                $('#LocationModal').modal('show');
                // get the locations
                $.ajax({
                    type: "POST",
                    url: "PMeetingSetUp.aspx/CheckAvailableLocations",
                    data: '{MeetingID:"' + MeetingID + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d != "") {
                            $('#<%=lbLocation.ClientID%> ').html(response.d);

                        }
                        else {
                            var str = '<option value="-1">No available location for meeting time and date</option>';
                            $('#<%=lbLocation.ClientID%> ').html(str);

                        }
                    },
                    error: function () {
                        PError('Error fetching available location, Try again!');

                    }
                });
                return false;
            }
        }
        function Show() {
            var chk = $('#rbResource').prop('checked');
            if (chk) {
                $('#divResource').show('slow');
                $('#divLocation').hide('slow');
            }
            else {
                $('#divResource').hide('slow');
                $('#divLocation').show('slow');
            }

        }
        function SaveSettings() {
            // after saving update the UI for resource
            var rchk = $('#rbResource').prop('checked');
            var lchk = $('#rbLocation').prop('checked');
            if (rchk) {
                // save resource
                saveResource();
            }
            if (lchk) {
                var location = $('#tbLocation').val();
                // save location
                $.ajax({
                    type: "POST",
                    url: "PMeetingSetUp.aspx/insertLocation",
                    data: '{name:"' + location + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $('#ResourceModal').modal('hide');
                    },
                    error: function () {
                        $('#ResourceModal').modal('hide');
                        PError('Error Saving Resource, Try again!');
                    }
                });
            }
        }
        // #endregion


        // Discussion Functions --------------------------------------------------------------------
        // #region Discussion
        function ShowDiscussion(obj) {
            DisID = $(obj).prop('title');
            $.ajax({
                type: "POST",
                url: "PMeetingSetUp.aspx/GetDiscussion",
                data: '{disId:"' + DisID + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d != "") {
                        $('#DiscussionInfo').html(response.d);
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
        function openDisModal() {
            DisModal = true;
            $('#DiscussionModal').modal('show');
        }
        function saveDiscussion() {
            if (MeetingID == 0) {
                PError('You need to save meeting information first!');
            }
            else {

                var title = $('#tbDisTitle').val();
                var text = $('#tbDisText').val();
                if (DisModal) {
                    $.ajax({
                        type: "POST",
                        url: "PMeetingSetUp.aspx/SaveDiscussion",
                        data: '{meetingID:"' + MeetingID + '", title:"' + title + '", text: "' + text + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response.d != "") {
                                $('#DiscussionModal').modal('hide');
                                $('#<%=DiscussionList.ClientID %>').prepend(response.d);
                            }
                            else {
                                $('#DiscussionModal').modal('hide');
                                PError('Error saving the meeting!');
                            }
                        },
                        error: function () {
                            $('#DiscussionModal').modal('hide');
                            PError('Error saving the meeting, Try again!');
                        }
                    });
                }
                else {
                    $.ajax({
                        type: "POST",
                        url: "PMeetingSetUp.aspx/SaveAgendaDiscussion",
                        data: '{AgendaID:"' + EditAgenda + '", title:"' + title + '", text: "' + text + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response.d != "") {
                                $('#DiscussionModal').modal('hide');
                                $('#<%=AgendaDiscussionList.ClientID %>').prepend(response.d);
                            }
                            else {
                                $('#DiscussionModal').modal('hide');
                                PError('Error saving the meeting!');
                            }
                        },
                        error: function () {
                            $('#DiscussionModal').modal('hide');
                            PError('Error saving the meeting, Try again!');
                        }
                    });
                }
            }
        }
        function GetDiscussions() {
            if (MeetingID != 0) {
                $.ajax({
                    type: "POST",
                    url: "PMeetingSetUp.aspx/GetDiscussions",
                    data: '{meetingID:"' + MeetingID + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d != "") {
                            $('#<%=DiscussionList.ClientID %>').append(response.d);
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
        }
        function saveReply() {
            var title = $('#tbDisCommentTitle').val();
            var text = $('#tbDisComment').val();
            $.ajax({
                type: "POST",
                url: "PMeetingSetUp.aspx/SaveReply",
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

        // Agenda Functions --------------------------------------------------------------------
        // #region Agenda

        // #region AgendaTable&Info
        function Data() {
            this.ID = 1;
            this.Text = "No Data";
        }
        function showAgendaIDs() {
            // loop through the table
            var t = $('#cbAgendaSub').prop('checked');
            if (t) {
                var IDList = new Array();
                $('#<%=AgendaTable.ClientID%> tbody>tr').each(function (index, tr) {
                    var $row = $(tr);
                    var i = new Data();
                    i.ID = $row.prop('id');
                    i.Text = $row.find('td:nth-child(1)').text();
                    if (i.Text != '') {
                        IDList.push(i);
                    }
                });
                $('#<%= lbAgendaSub.ClientID%>').show('slow');
                $('#<%= lbAgendaSub.ClientID%>').empty();
                for (var i = 0; i < IDList.length; i++) {
                    var text = '<option value="' + IDList[i].ID + '">' + IDList[i].Text + ' </option>';
                    $('#<%= lbAgendaSub.ClientID%>').append(text);
                }
            }
            else {
                $('#<%= lbAgendaSub.ClientID%>').hide('slow');
            }
        }
        function SaveAgendaInfo(publish) {
            var id = $('#<%= tbAgendaID.ClientID%>').val();
            var title = $('#tbAgendaTitle').val();
            var desc = $('#tbAgendaDesc').val();
            var t = $('#cbAgendaSub').prop('checked');
            var sub = 0;
            if (t) {
                sub = $('#<%=lbAgendaSub.ClientID %> option:selected').val();
            }
            if (MeetingID == 0) {
                PError('You need to save meeting information first!');
                return false;
            }
            if (EditAgenda != 0) {
                // update information
                $.ajax({
                    type: "POST",
                    url: "PMeetingSetUp.aspx/UpdateAgenda",
                    data: '{agendaID: "' + EditAgenda + '", count:"' + id + '", title:"' + title + '", desc:"' + desc + '", subagenda:"' + sub + '", published:"' + publish + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d == "true") {
                            fetchAgenda();
                            $('#<%= tbAgendaID.ClientID%>').val("");
                            $('#tbAgendaTitle').val("");
                            $('#tbAgendaDesc').val("");
                            $('#cbAgendaSub').prop('checked', false);
                            $('#<%=lbAgendaSub.ClientID %> ').hide('slow');
                        }
                        else {
                            PError('Error saving Agenda!');
                        }
                    },
                    error: function () {
                        PError('Error saving agenda, Try again!');
                    }
                });
            }
            else {
                // save information
                $.ajax({
                    type: "POST",
                    url: "PMeetingSetUp.aspx/SaveAgenda",
                    data: '{count:"' + id + '", title:"' + title + '", desc:"' + desc + '", meetingid:"' + MeetingID + '", subagenda:"' + sub + '", published:"' + publish + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d == "true") {
                            fetchAgenda();
                            $('#<%= tbAgendaID.ClientID%>').val("");
                            $('#tbAgendaTitle').val("");
                            $('#tbAgendaDesc').val("");
                            $('#cbAgendaSub').prop('checked', false);
                            $('#<%=lbAgendaSub.ClientID %> ').hide('slow');
                        }
                        else {
                            PError('Error saving Agenda!');
                        }
                    },
                    error: function () {
                        PError('Error saving agenda, Try again!');
                    }
                });
            }
        }

        function fetchAgenda() {

            if (MeetingID != 0) {
                $.ajax({
                    type: "POST",
                    url: "PMeetingSetUp.aspx/FetchAgenda",
                    data: '{meetingID:"' + MeetingID + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d != "") {
                            $('#<%=AgendaTable.ClientID %>').find("tr:gt(0)").remove();
                            $('#<%=AgendaTable.ClientID %> > tbody:last').append(response.d);
                        }
                        else {
                            Error('No Agenda yet!');
                        }
                    },
                    error: function () {
                        PError('Error fetching data, Try again!');
                    }
                });
            }
        }

        function highlightRow(obj) {
            // remoe highlight from other rows
            $('#<%=AgendaTable.ClientID %> tr').each(function (index, tr) {
                var $row = $(tr);
                $row.removeClass("info");
            });
            $row = $(obj);
            $row.addClass("info");
        }
        function AgendaIconClick(ID, Type) {
            highlightRow();
            // open the tab based on the type and get id data
            switch (Type) {
                case 'Edit':
                    AgendaForEdit(ID);
                    return false;
                case 'Delete':
                    break;
                case 'Privacy':
                    AgendaPrivacyEdit(ID);
                    return false;
                case 'Discussion':
                    GetAgendaDiscussions(ID);
                    return false;
                case 'File':
                    break;
                case 'Task':
                    AgendaTaskEdit(ID);
                    return false;
            }
        }

        function AgendaUI(creator) {
            if (creator == 'False') {
                // Info
                $('#AgendaInfo').remove();
                $('#AgendaInfoTitle').remove();
                // privacy
                $('#AgendaPrivacy').remove();
                $('#AgendaPrivacyTitle').remove();
            }
        }

        // functions to activate the tab and load information
        function AgendaForEdit(id) {
            if (id != 0) {
                EditAgenda = id;
            }
            else {
                if (EditAgenda == 0) {
                    ResetAgendaPanelsUI();
                    return false;
                }
            }
            // load data into controls from servers
            $.ajax({
                type: "POST",
                url: "PMeetingSetUp.aspx/GetAgenda",
                data: '{gid:"' + EditAgenda + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var result = response.d;
                    $('#<%= tbAgendaID.ClientID%>').val(result.Count);
                    $('#tbAgendaTitle').val(result.Title);
                    $('#tbAgendaDesc').val(result.Desc);
                    if (result.Parent == 0) {
                        // not sub
                        $('#cbAgendaSub').prop('checked', false);
                        $('#<%=lbAgendaSub.ClientID %> ').hide('slow');
                    }
                    else {
                        $('#cbAgendaSub').prop('checked', true);
                        showAgendaIDs();
                        $('#<%=lbAgendaSub.ClientID %> option[value="' + result.Parent + '"]').attr("selected", true);
                    }
                },
                error: function () {
                    PError('Error fetching agenda information, Try again!');
                }
            });

            $('#AgendaTab a[href="#AgendaInfo"]').tab('show');
            $('#btnAgendaReset').prop('hidden', false);
        }
        //TODO: complete resetting the panels
        function ResetAgendaPanels() {
            // reset variables
            EditAgenda = 0;
            ResetAgendaPanelsUI();
        }

        function ResetAgendaPanelsUI() {
            // info panel
            $('#<%= tbAgendaID.ClientID%>').val(''); 0
            $('#tbAgendaTitle').val('');
            $('#tbAgendaDesc').val('');
            $('#cbAgendaSub').prop('checked', false);
            $('#<%=lbAgendaSub.ClientID %> ').hide('slow');
            $('#btnAgendaReset').prop('hidden', true);
            // Document Panel
            // Privacy Panel
            $('#AgendaPrivacyTable').find("tr:gt(0)").remove();
            // Discussion panel
            // Tasks panel
        }
        // #endregion

        // #region Agenda Privacy
        function AgendaPrivacyEdit(ID) {
            if (ID != 0) {
                EditAgenda = ID;
            }
            else {
                if (EditAgenda == 0) {
                    ResetAgendaPanelsUI();
                    return false;
                }
            }

            $.ajax({
                type: "POST",
                url: "PMeetingSetUp.aspx/GetAgendaPrivacy",
                data: '{AgendaID:"' + EditAgenda + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var result = response.d;
                    $('#AgendaPrivacyTable').find("tr:gt(0)").remove();
                    $('#AgendaPrivacyTable > tbody:last').append(result);

                },
                error: function () {
                    PError('Error fetching agenda privacy information, Try again!');
                }
            });
            $('#AgendaTab a[href="#AgendaPrivacy"]').tab('show');
        }

        function UpdateAgendaPrivacy() {
            if (EditAgenda != 0) {
                var UserList = new Array();
                $('#AgendaPrivacyTable > tbody tr').each(function (index, tr) {
                    var $row = $(tr);
                    var username = $row.find('td:nth-child(1)').text();
                    var canSee = $row.find('input:checkbox')[0].checked;
                    var user = new Privacy(username, canSee);
                    UserList.push(user);
                });

                $.ajax({
                    type: "POST",
                    url: "PMeetingSetUp.aspx/UpdateAgendaPrivacy",
                    data: '{obj:' + JSON.stringify(UserList) + ', agendaID:"' + EditAgenda + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        alert('true');
                    },
                    error: function () {
                        PError('Error updating privacy information, Try again!');
                    }
                });
            }
        }
        // #endregion

        // #region Agenda Task Assignment
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
        function SaveAgendaTask() {
            if (EditAgenda != 0) {
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
                if (_EditAgendaTask == 0) {
                    // call server /save
                    $.ajax({
                        type: "POST",
                        url: "PMeetingSetUp.aspx/SaveAgendaTask",
                        data: '{AgendaID:"' + EditAgenda + '", Title:"' + title + '", Desc:"' + desc + '", DueDate:"' + date + '", usernames:' + JSON.stringify(users) + '}',
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
                            PError('Error saving agenda task, Try again!');
                        }
                    });
                }
                else {
                    // update
                    $.ajax({
                        type: "POST",
                        url: "PMeetingSetUp.aspx/UpdateAgendaTask",
                        data: '{TaskID: "' + _EditAgendaTask + '", AgendaID:"' + EditAgenda + '", Title:"' + title + '", Desc:"' + desc + '", DueDate:"' + date + '", usernames:' + JSON.stringify(users) + '}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            var txt = response.d;
                            var parent = $('#div' + _EditAgendaTask);
                            parent.replaceWith(txt);
//                            var parent = $('#div' + _EditAgendaTask);
//                            parent.html('');
//                            $(txt).hide().appendTo(parent).show('slow');

                            NewTaskEntry();
                        },
                        error: function () {
                            PError('Error saving agenda task, Try again!');
                        }
                    });
                }
            }
        }

        function GetAgendaTasks() {
            if (EditAgenda == 0) {
                return false;
            }

            else {
                $.cookie('Agenda', EditAgenda);
                $.ajax({
                    type: "POST",
                    url: "PMeetingSetUp.aspx/GetAgendaTasks",
                    data: '{AgendaID:"' + EditAgenda + '"}',
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
            }
        }
        function AgendaTaskEdit(id) {
            if (id != 0) {
                EditAgenda = id;
            }
            $('#AgendaTab a[href="#AgendaTask"]').tab('show');
            GetAgendaTasks();
        }
        function EditAgendaTask(obj) {
            var id = $(obj).prop("id").split('.')[1];
            _EditAgendaTask = id;
            $.ajax({
                type: "POST",
                url: "PMeetingSetUp.aspx/GetAgendaTask",
                data: '{TaskID:"' + id + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var result = response.d;
                    $('#btnAction').text(result.Title);
                    $('#tbTaskDesc').val(result.Defsc);
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
        function DeleteAgendaTaskConfirm(obj) {
            var id = $(obj).prop("id").split('.')[1];
            ConfirmTask('Delete Task', 'Are you sure you want to delete the task?', id);
            return false;
        }
        function DeleteAgendaTask(id) {
            $.ajax({
                type: "POST",
                url: "PMeetingSetUp.aspx/DeleteAgendaTask",
                data: '{TaskID:"' + id + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var result = response.d;
                    if (result == "true") {
                        GetAgendaTasks();
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
            _EditAgendaTask = 0;
        }

        function EditUserTaskResponse(obj) {
            var id = $(obj).prop("id").split('.')[1];
            var PDiv = $(obj).parent().parent();
            $.ajax({
                type: "POST",
                url: "PMeetingSetUp.aspx/GetUserTaskResponse",
                data: '{TaskID:"' + id + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var result = response.d;
                    if (result != "") {
//                        var parent = $('#div' + id + ' > .row-fluid ');
//                        parent.find('hr:first-child').remove();
//                        parent.find('div:first-child').remove();
//                        $(result).hide().appendTo(parent).show('slow');

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
                url: "PMeetingSetUp.aspx/SaveTaskResponse",
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
            var parent = $('#divTool' + id).parent();
            $(result).hide().appendTo(parent).show('slow');
            // hide the tool
            $('#divTool' + id).prev('hr').remove();

            $('#divTool' + id).hide('slow').remove();

//            var parent = $('#div' + id);
//            $(result).hide().appendTo(parent).show('slow');
//            // hide the tool
//            $('#divTool' + id).prev('hr').hide();
//            $('#divTool' + id).hide('slow');
        }
        // #endregion

        // #region Agenda Discussion

        function ShowAgendaDiscussion(obj) {
            DisID = $(obj).prop('title');
            $.ajax({
                type: "POST",
                url: "PMeetingSetUp.aspx/GetAgendaDiscussion",
                data: '{disId:"' + DisID + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d != "") {
                  
                        $('#AgendaDiscussionInfo').html(response.d);
                        $('#AgendaDiscussionPreview').show('slow');
                        $('#AddAgendaComment').show();
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
        function openAgendaDisModal() {
            DisModal = false;
            $('#DiscussionModal').modal('show');
        }
        function GetAgendaDiscussions(id) {
            if (id != 0)
            {EditAgenda = id;}
            if (EditAgenda != 0) {
                $.ajax({
                    type: "POST",
                    url: "PMeetingSetUp.aspx/GetAgendaDiscussions",
                    data: '{AgendaID:"' + EditAgenda + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d != "") {
                            $('#<%=AgendaDiscussionList.ClientID %>').html(response.d);
                            $('#AgendaDiscussionPreview').hide();
                        }
                        else {
                            $('#<%=AgendaDiscussionList.ClientID %>').html("<p>No Discussions</p>");
                            $('#AgendaDiscussionPreview').hide();
                        }
                    },
                    error: function () {
                        PError('Error fetching agenda discussions, Try again!');
                    }
                });
                $('#AgendaTab a[href="#AgendaDiscussion"]').tab('show');
            }
        }
        function saveAgendaReply() {
            var title = $('#tbAgendaDisCommentTitle').val();
            var text = $('#tbAgendaDisComment').val();
            $.ajax({
                type: "POST",
                url: "PMeetingSetUp.aspx/SaveReply",
                data: '{disID:"' + DisID + '", title:"' + title + '", text:"' + text + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d != "") {
                        $('#AgendaReplies').append(response.d);
                        $('#tbAgendaDisCommentTitle').val('');
                        $('#tbAgendaDisComment').val('');
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
        // #endregion 
        
        
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="updatepanel1" runat="server">
        <ContentTemplate>
            <div class="container">
                <div class="row-fluid">
                    <div class=" navbar navbar-static-top  span12">
                        <div class="tabs-left">
                            <ul class="nav nav-tabs" id="myTab">
                                <li class="active"><a href="#Info" data-toggle="tab">Meeting Info</a></li><br />
                                <li><a href="#Participants" data-toggle="tab" onclick="showParti()">Participants</a></li><br />
                                <li><a href="#Resources" data-toggle="tab" onclick="FetchResources()">Resources</a></li><br />
                                <li><a href="#Discussion" data-toggle="tab">Discussion</a></li><br />
                                <li><a href="#Agenda" data-toggle="tab" onclick="fetchAgenda()">Agenda</a></li><br />
                            </ul>
                        </div>
                        <div class="tab-content">
                            <div class="tab-pane active" id="Info">
                                <h2>
                                    Meeting Information
                                </h2>
                                <div class="offset6">
                                    <a id="aSave" class="win-command pull-right" title="Save" href="#" onclick="return SaveInfo()">
                                        <span class="win-commandicon win-commandring icon-checkmark"></span></a>
                                </div>
                                <div id="ProgressInfo" style="display: none" class="progress progress-indeterminate pull-right">
                                    <div class="win-ring small">
                                    </div>
                                </div>
                                <hr />
                                <div class="offset1">
                                    <br />
                                    <br />
                                    <div id="divResult" class="alert alert-success" hidden="hidden">
                                        <button class="close" data-dismiss="alert" type="button">
                                        </button>
                                        <strong>Saved!</strong>
                                    </div>
                                    <table>
                                        <tr>
                                            <td style="width: 150px">
                                                Meeting ID
                                            </td>
                                            <td>
                                                <input type="text" runat="server" id="tbID" readonly="readonly" style="width: 100%" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Meeting Title
                                            </td>
                                            <td>
                                                <input type="text" runat="server" id="tbtitle" style="width: 100%" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Meeting Type
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlMType" runat="server" Style="width: 100%">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Start Date
                                            </td>
                                            <td>
                                                <input type="text" runat="server" id="tbSDate" class="datepicker" style="width: 100%" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                End Date
                                            </td>
                                            <td>
                                                <input type="text" runat="server" id="tbEDate" class="datepicker" style="width: 100%" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                All Day
                                            </td>
                                            <td>
                                                <label class="checkbox inline">
                                                    <input type="checkbox" id="cbAllDay" value="2" onclick="AllDay()"><span style="width: 100px"
                                                        class="metro-checkbox">All Day Event</span>
                                                </label>
                                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Start Time
                                            </td>
                                            <td>
                                                <input id="tbSTime" type="text" value="8:00" runat="server" style="width: 100%"></input>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                End Time
                                            </td>
                                            <td>
                                                <input type="text" id="tbETime" value="10:00" runat="server" style="width: 100%" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Description
                                            </td>
                                            <td>
                                                <textarea id="tbDesc" rows="2" cols="2" runat="server"></textarea>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                            <div class="tab-pane" id="Participants">
                                <h2>
                                    Participants</h2>
                                <div class="offset6">
                                    <a id="A2" class="win-command pull-right" href="#" onclick="return SaveParticipants()"
                                        title="Save Participants"><span class="win-commandicon win-commandring icon-checkmark">
                                        </span></a>
                                </div>
                                <div id="ProgressParti" style="display: none" class="progress progress-indeterminate pull-right">
                                    <div class="win-ring small">
                                    </div>
                                </div>
                                <hr />
                                <div class="container-fluid">
                                    <div class="row-fluid">
                                        <div class="span11" style="border: 2px solid #f5f5f5">
                                            <h4 style="padding-left: 5px; padding-top: 10px">
                                                Add Participants</h4>
                                            <hr />
                                            <div class="row-fluid">
                                                <div class="span4">
                                                    <p>
                                                        Participant</p>
                                                    <asp:ListBox ID="lbPart" runat="server" Width="200px"></asp:ListBox>
                                                </div>
                                                <div class="span4">
                                                    <p>
                                                        Role</p>
                                                    <asp:ListBox ID="lbRole" runat="server" Width="200px">
                                                        <asp:ListItem>Attendee</asp:ListItem>
                                                        <asp:ListItem>Chairman</asp:ListItem>
                                                    </asp:ListBox>
                                                </div>
                                                <div class="span4">
                                                    <p>
                                                        Notes</p>
                                                    <textarea id="tbNote" cols="" rows="3"></textarea></div>
                                            </div>
                                            <div class="row-fluid">
                                                <button class="btn-small btn-primary pull-right" type="button" onclick="AddParticipant()">
                                                    Add</button>
                                            </div>
                                        </div>
                                        <br />
                                        <br />
                                    </div>
                                    <div class="row-fluid">
                                        <div class="span11" style="overflow-x: hidden; overflow: scroll; height: 400px">
                                            <h4 style="padding-left: 5px; padding-top: 10px">
                                                Meeting Participants</h4>
                                            <hr />
                                            <table class="table table-hover" id="parTable" runat="server">
                                                <thead>
                                                    <tr>
                                                        <th>
                                                            #
                                                        </th>
                                                        <th>
                                                            User
                                                        </th>
                                                        <th>
                                                            Role
                                                        </th>
                                                        <th>
                                                            Response
                                                        </th>
                                                        <th>
                                                            Notes
                                                        </th>
                                                        <th>
                                                        </th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                </tbody>
                                            </table>
                                            <br />
                                            <div class="row-fluid">
                                                <div id="SuccessParti" class="alert alert-success" style="display: none;">
                                                    <button class="close" data-dismiss="alert" type="button">
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="tab-pane" id="Resources">
                                <h2>
                                    Resources <a id="btnResourceSetting" class="win-command" title="Meeting Resource Settings"
                                        href="#ResourceModal" style="display: none" data-toggle="modal"><span class="win-commandicon win-command icon-cog-3">
                                        </span></a>
                                </h2>
                                <div class="offset6">
                                    <a id="btnSave" class="win-command pull-right" href="" title="Next" runat="server"><span
                                        class="win-commandicon win-commandring icon-last"></span></a>
                                </div>
                                <hr />
                                <div class="container-fluid offset1">
                                    <div class="row-fluid">
                                        <div>
                                            <table style="width: 100%">
                                                <tr>
                                                    <td style="width: 30%">
                                                        <h4>
                                                            Meeting Venue</h4>
                                                    </td>
                                                    <td id="MVenueTD" style="width: 40%">
                                                    </td>
                                                    <td style="width: 30%">
                                                        <a style="font-size: smaller" id="UpdateLocation" href="#" onclick="return GetAvailableLocation()">
                                                            Update location</a>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <hr />
                                        <div>
                                            <table style="width: 100%">
                                                <tr>
                                                    <td style="width: 30%; vertical-align: top">
                                                        <h4>
                                                            Meeting Resources</h4>
                                                    </td>
                                                    <td style="width: 40%">
                                                        <div id="divItems" runat="server">
                                                        </div>
                                                    </td>
                                                    <td style="vertical-align: bottom; width: 30%">
                                                        <a href="#" onclick="return UpdateResources()" style="font-size: smaller">Update resources</a>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <hr />
                                    </div>
                                    <div class="row-fluid">
                                        <br />
                                        <hr />
                                        <br />
                                    </div>
                                </div>
                                <hr />
                            </div>
                            <div class="tab-pane" id="Discussion">
                                <h2>
                                    Discussions</h2>
                                <hr />
                                <div class="container-fluid">
                                    <div class="row-fluid">
                                        <div style="height: 250px; overflow-x: hidden; overflow: auto; background-color: ">
                                            <input class="btn-mini btn-info pull-right" type="button" onclick="openDisModal()"
                                                value="Add New Discussion">
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
                            <div class="tab-pane" id="Agenda">
                                <h2>
                                    Agenda</h2>
                                <div class="offset6">
                                </div>
                                <hr />
                                <div class="container-fluid">
                                    <div class="row-fluid" style="overflow: auto; height: 200px">
                                        <table class="table table-hover" id="AgendaTable" runat="server">
                                            <thead>
                                                <tr>
                                                    <th>
                                                        ID
                                                    </th>
                                                    <th>
                                                        Title
                                                    </th>
                                                    <th>
                                                        Describtion
                                                    </th>
                                                    <th>
                                                        Status
                                                    </th>
                                                    <th>
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        1
                                                    </td>
                                                    <td>
                                                        introduction
                                                    </td>
                                                    <td>
                                                        lsdmgsdngkajsdg;alk
                                                    </td>
                                                    <td>
                                                        Published
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        22
                                                    </td>
                                                    <td>
                                                        introduction
                                                    </td>
                                                    <td>
                                                        lsdmgsdngkajsdg;alk
                                                    </td>
                                                    <td>
                                                        Published
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                    <hr />
                                    <div class="row-fluid navbar navbar-inverse">
                                        <div class="navbar-inner">
                                            <ul class="nav" id="AgendaTab">
                                                <li id="AgendaInfoTitle" class="active"><a href="#AgendaInfo" onclick="AgendaForEdit(0)"
                                                    data-toggle="tab">Agenda Info</a></li>
                                                <li id="AgendaPrivacyTitle" class="divider-vertical"><a href="#AgendaPrivacy" data-toggle="tab"
                                                    onclick="AgendaPrivacyEdit(0)">Privacy Settings</a></li>
                                                <li class="divider-vertical"><a href="#AgendaDocument" data-toggle="tab">Documents</a></li>
                                                <li class="divider-vertical"><a onclick="GetAgendaDiscussions(0)" href="#AgendaDiscussion" data-toggle="tab">Discussion</a></li>
                                                <li class="divider-vertical"><a href="#AgendaTask" data-toggle="tab" onclick="GetAgendaTasks()">
                                                    Task Assignments</a></li>
                                            </ul>
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="tab-content">
                                            <div class="tab-pane active" id="AgendaInfo">
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td style="width: 100px">
                                                            ID
                                                        </td>
                                                        <td>
                                                            <input id="tbAgendaID" runat="server" type="text" pattern="^\d+(\.\d+)*$" />
                                                        </td>
                                                        <td>
                                                            <div class="pull-right">
                                                                <input id="btnAgendaReset" type="button" value="New Agenda" onclick="ResetAgendaPanels()"
                                                                    hidden="hidden" />
                                                                <input id="btnAgendaInfoSave" class="btn-primary" type="button" value="Save" onclick="SaveAgendaInfo(false)" />
                                                                <input id="btnAgendaInfoPublish" class="btn-primary" type="button" value="Publish"
                                                                    onclick="SaveAgendaInfo(true)" />
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Title
                                                        </td>
                                                        <td>
                                                            <input id="tbAgendaTitle" type="text" />
                                                        </td>
                                                        <td>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Description
                                                        </td>
                                                        <td>
                                                            <textarea id="tbAgendaDesc" rows="" cols=""></textarea>
                                                        </td>
                                                        <td>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Sub Agenda
                                                        </td>
                                                        <td>
                                                            <label class="checkbox inline">
                                                                <input type="checkbox" id="cbAgendaSub" value="0" onclick="showAgendaIDs()"><span
                                                                    style="width: 100px" class="metro-checkbox"></span>
                                                            </label>
                                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br />
                                                            <br />
                                                            <asp:ListBox ID="lbAgendaSub" runat="server" Style="display: none;"></asp:ListBox>
                                                        </td>
                                                        <td>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <div class="tab-pane" id="AgendaDocument">
                                                agenda documents
                                            </div>
                                            <div class="tab-pane" id="AgendaDiscussion">
                                                <h4>
                                                    Agenda Discussions
                                                </h4>
                                                <hr />
                                                <div class="container-fluid">
                                                    <div class="row-fluid">
                                                        <div style="height: 250px; overflow-x: hidden; overflow: auto; background-color: ">
                                                            <input class="btn-mini btn-info pull-right" type="button" onclick="openAgendaDisModal()"
                                                                value="Add New Discussion">
                                                            <br />
                                                            <div id="AgendaDiscussionList" runat="server">
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row-fluid">
                                                        <div id="AgendaDiscussionPreview" class="span11">
                                                            <div id="AgendaDiscussionInfo">
                                                            </div>
                                                            <div id="AddAgendaComment" class="container-fluid" style="display: none">
                                                                <div class="row-fluid replyBox">
                                                                    <div class=" replyContent">
                                                                        <input type="text" id="tbAgendaDisCommentTitle" style="width: 450px" class="input-block-level" placeholder="Title" />
                                                                        <textarea id="tbAgendaDisComment" rows="0" cols="15" class="input-block-level" placeholder="Insert a comment"></textarea>
                                                                        <input id="btnAgendaDisComment" type="button" value="Send" onclick="saveAgendaReply()" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="tab-pane" id="AgendaPrivacy">
                                                <div class="pull-right">
                                                    <input id="btnSaveAgendaPrivacy" type="button" class="btn-primary" value="Save" onclick="UpdateAgendaPrivacy()" />
                                                </div>
                                                <br />
                                                <table class="table table-striped" style="width: 100%" id="AgendaPrivacyTable">
                                                    <thead>
                                                        <tr>
                                                            <th>
                                                                Meeting Participants
                                                            </th>
                                                            <th>
                                                                Role
                                                            </th>
                                                            <th>
                                                                Can See?
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                    </tbody>
                                                </table>
                                            </div>
                                            <div class="tab-pane" id="AgendaTask">
                                                <h4>
                                                    Agenda Tasks</h4>
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
                                                                <td style="border: 1px solid red; background-color:#FF4D4D; color:White">
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
                                                            <input type="button" class="btn-primary" value="Save" onclick="SaveAgendaTask()" /><input
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
                                                                        <asp:TextBox ID="tbAgendaTaskBy" Width="185px" runat="server"></asp:TextBox><asp:AutoCompleteExtender
                                                                            ID="AutoCompleteExtender1" runat="server" TargetControlID="tbAgendaTaskBy" OnClientItemSelected="AddTaskParti"
                                                                            UseContextKey="True" MinimumPrefixLength="1" ServiceMethod="GetCompletionList"
                                                                            EnableCaching="false">
                                                                        </asp:AutoCompleteExtender>
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
                                                                                    <td style="width: 150px">
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
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Modals-->
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
                <div aria-hidden="true" aria-labelledby="myModalLabel" role="dialog" tabindex="-1"
                    class="modal info bg-color-green hide fade" id="MyModalSuccess" style="display: none;">
                    <div class="modal-body">
                        <p id="pSuccess">
                        </p>
                    </div>
                    <div class="modal-footer">
                        <button data-dismiss="modal" class="btn btn-large">
                            Close</button>
                    </div>
                </div>
                <div id="ResourceModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
                    aria-hidden="true">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                        </button>
                        <h3 id="H1">
                            Settings</h3>
                    </div>
                    <div class="modal-body">
                        <label class="radio">
                            <input type="radio" name="optionsRadios" id="rbResource" value="0" checked onclick="Show()">
                            <span class="metro-radio">Add New Resource</span>
                        </label>
                        &nbsp;&nbsp;&nbsp;&nbsp;<label class="radio"><input type="radio" name="optionsRadios" id="rbLocation" value="1" onclick="Show()">
                            <span class="metro-radio">Add New Location</span>
                        </label>
                        &nbsp;&nbsp;&nbsp;&nbsp;<hr /><div id="divResource">
                            <table>
                                <tr>
                                    <td style="width: 150px">
                                        Resource
                                    </td>
                                    <td>
                                        <input type="text" id="tbResourceName" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Describtion
                                    </td>
                                    <td>
                                        <input type="text" id="tbResourceDesc" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="divLocation" hidden="hidden">
                            <table>
                                <tr>
                                    <td style="width: 150px">
                                        Location
                                    </td>
                                    <td>
                                        <input type="text" id="tbLocation" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button class="btn" data-dismiss="modal">
                            Close</button>
                        <input type="button" id="tbSave" class="btn btn-primary" onclick="SaveSettings()"
                            value="Save" />
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
                <div id="LocationModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
                    aria-hidden="true">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                        </button>
                        <h3>
                            Set Meeting Venue</h3>
                        <hr />
                    </div>
                    <div class="modal-body">
                        <h4>
                            Available locations</h4>
                        <br />
                        <asp:ListBox ID="lbLocation" runat="server" Width="400px"></asp:ListBox>
                    </div>
                    <div class="modal-footer">
                        <button class="btn" data-dismiss="modal">
                            Close</button>
                        <input type="button" id="btnSaveLocation" class="btn btn-primary" onclick="UpdateLocation()"
                            value="Save" />
                    </div>
                </div>
            </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
