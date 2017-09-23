<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="PCalender.aspx.cs" Inherits="TheBox.PCalender" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel='stylesheet' href="content/cupertino/theme.css" />
    <link href="content/fullcalendar/fullcalendar.css" rel='stylesheet' />
    <link href="content/fullcalendar/fullcalendar.print.css" rel='stylesheet' media='print' />
    <script src="content/jquery/jquery-1.9.1.min.js"></script>
    <script src='content/jquery/jquery-ui-1.10.2.custom.min.js'></script>
    <script src='content/fullcalendar/fullcalendar.min.js'></script>
    <script>
        $(document).ready(function () {
            $('#calendar').fullCalendar({
                theme: true,
                header: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'month,agendaWeek,agendaDay'
                },
                editable: true,
                selectable: true,
                selectHelper: true,
                events: { url: "Events.aspx", borderColor: "#7C95AD" },
                dayClick: NewEntry,
                eventClick: EditEvent,
                select: NewDayEntry
            });
        });



        function NewDayEntry(startDate, endDate, allDay, jsEvent, view) {
            resetControls();
            $('#myModal').modal('show');
            $('#tbFrmDate').val(formatDate(startDate));
            $('#tbToDate').val(formatDate(endDate));
            $('#tbFrmTime').val(formatTime(startDate));
            $('#tbToTime').val(formatTime(endDate));
            enableCBs();
            if (allDay) {
                $('#cbAllDay').prop('checked', true);
                AllDay();
            }
            else {
                $('#cbAllDay').prop('checked', false);
                AllDay();
            }
        }
        function NewEntry(date, allDay, jsEvent, view) {
            resetControls();
            if (view.name == "month") {
                $('#myModal').modal('show');
                $('#tbFrmDate').val(formatDate(date));
                $('#tbToDate').val(formatDate(date));
                enableCBs();
            }
        }
        function resetControls() {
            $('#tbFrmDate').val("");
            $('#tbToDate').val("");
            $('#tbFrmTime').val("");
            $('#tbToTime').val("");
            $('#cbAllDay').prop('checked', false);
            $('#tbTitle').val("");
            $('#tbMeeDesc').val("");
            $('#tbAppdesc').val("");
            $('#<%=tbAppPerson.ClientID%>').val("");
            $('#<%=ddlType.ClientID%>').prop('selectedIndex', 0);
            $('#rbCal').prop('checked', 'checked');
            $('#myModalLabel').html('New Event');
            $('#trID').prop('hidden', 'hidden');
            showCal();
            $('#tbSave').val('Save');
            $('#tbDelete').hide();
        }
        function enableCBs() {
            $('#rbApp').prop('disabled', false);
            $('#rbCal').prop('disabled', false);
            $('#rbMee').prop('disabled', false);

        }
        function formatDate(date) {
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

        // modal functions
        function showMee() {
            $('#MeetingModal').show();
            $('#AppointmentModal').hide();
        }

        function showApp() {
            $('#AppointmentModal').show();
            $('#MeetingModal').hide();
        }
        function showCal() {
            $('#AppointmentModal').hide();
            $('#MeetingModal').hide();
        }
        function AllDay() {
            if ($('#cbAllDay').is(':checked')) {

                $('#tbToTime').prop("disabled", true);
            }
            else {

                $('#tbToTime').prop("disabled", false);
            }
        }

        function SaveEntry() {
            var edit = $('#trID').is(':hidden');
            var fromdate = $('#tbFrmDate').val();
            var fromtime = $('#tbFrmTime').val();
            var title = $('#tbTitle').val();
            var toDate = $('#tbToDate').val();
            var toTime;
            var day = $('#cbAllDay').is(':checked');
            if (day) {
                toTime = "00:00:00";
            }
            else {
                toTime = $('#tbToTime').val();
            }

            var type = -1;
            if ($('#rbCal').is(':checked')) {
                type = $('#rbCal').val();
            }
            if ($('#rbMee').is(':checked')) {
                type = $('#rbMee').val();
            }
            if ($('#rbApp').is(':checked')) {
                type = $('#rbApp').val();
            }
            switch (type) {
                case "0":
                    if (!edit) {
                        var id = $('#tbID').val();
                        $.ajax({
                            type: "POST",
                            url: "PCalender.aspx/UpdateCalendar",
                            data: '{id:"'+id+'", sdate:"' + fromdate + '", edate:"' + toDate + '", stime:"' + fromtime + '", etime:"' + toTime + '", etitle:"' + title + '",aday:"' + day + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (response) {
                                $('#calendar').fullCalendar('refetchEvents');
                                $('#myModal').modal('hide');
                            }
                        });
                    }
                    else {
                        $.ajax({
                            type: "POST",
                            url: "PCalender.aspx/SaveCal",
                            data: '{sdate:"' + fromdate + '", edate:"' + toDate + '", stime:"' + fromtime + '", etime:"' + toTime + '", etitle:"' + title + '",aday:"' + day + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (response) {
                                $('#calendar').fullCalendar('refetchEvents');
                                $('#myModal').modal('hide');
                            }
                        });
                    }
                    break;
                case "1":
                    var descApp = $('#tbAppdesc').val();
                    var invited = $('#<%= tbAppPerson.ClientID %>').val();
                    if (!edit) {
                        var id = $('#tbID').val();
                        $.ajax({
                            type: "POST",
                            url: "PCalender.aspx/UpdateAppointment",
                            data: '{id:"' + id + '", sdate:"' + fromdate + '", edate:"' + toDate + '", stime:"' + toTime + '", etime:"' + toTime + '", etitle:"' + title + '", pinv:"' + invited + '",aday:"' + day + '",desc:"' + descApp + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (response) {
                                $('#calendar').fullCalendar('refetchEvents');
                                $('#myModal').modal('hide');
                            }
                        });
                    }
                    else {
                        $.ajax({
                            type: "POST",
                            url: "PCalender.aspx/SaveApp",
                            data: '{sdate:"' + fromdate + '", edate:"' + toDate + '", stime:"' + toTime + '", etime:"' + toTime + '", etitle:"' + title + '", pinv:"' + invited + '",aday:"' + day + '",desc:"' + descApp + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (response) {
                                $('#calendar').fullCalendar('refetchEvents');
                                $('#myModal').modal('hide');
                            }
                        });
                    }
                    break;
                case "2":
                    var descMee = $('#tbMeeDesc').val();
                    var MeeType = $('#<%=ddlType.ClientID %> option:selected').val();
                    if (!edit) {
                        var id = $('#tbID').val();
                        $.ajax({
                            type: "POST",
                            url: "PCalender.aspx/UpdateMeeting",
                            data: '{id:"' + id + '", sdate:"' + fromdate + '", edate:"' + toDate + '", stime:"' + fromtime + '", etime:"' + toTime + '", etitle:"' + title + '", desc:"' + descMee + '", mtype:"' + MeeType + '", aday:"' + day + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (response) {
                                $('#calendar').fullCalendar('refetchEvents');
                                $('#myModal').modal('hide');
                                var url = "PMeetingSetUp.aspx?id=" + id;
                                window.location.href = url;
                            }
                        });
                    }
                    else {
                        $.ajax({
                            type: "POST",
                            url: "PCalender.aspx/SaveMeet",
                            data: '{sdate:"' + fromdate + '", edate:"' + toDate + '", stime:"' + fromtime + '", etime:"' + toTime + '", etitle:"' + title + '", desc:"' + descMee + '", mtype:"' + MeeType + '", aday:"' + day + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (response) {
                                $('#calendar').fullCalendar('refetchEvents');
                                $('#myModal').modal('hide');
                            }
                        });
                    }
                    break;
            }
        }
        function filter() {
            var meet = $('#<%=cbMeet.ClientID%>').is(':checked');
            var cal = $('#<%=cbCal.ClientID%>').is(':checked');
            var app = $('#<%=cbApp.ClientID%>').is(':checked');
            $.ajax({
                type: "POST",
                url: "Events.aspx/FilterEvents",
                data: '{Cal:"' + cal + '", App:"' + app + '", Meet:"' + meet + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    $('#calendar').fullCalendar('refetchEvents');
                },
                error: function (xhr, status, ll) {
                    alert('error');
                }
            });
        }

        function EditEvent(calEvent, jsEvent, view) {

            var id = calEvent.id;
            $.ajax({
                type: "POST",
                url: "PCalender.aspx/GetEvent",
                data: '{id:"' + id + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var e = response.d;
                    ShowEvent(e);
                },
                error: function (xh, status, ll) {
                    alert(xh.status);
                    alert(ll);

                }
            });
        }

        function ShowEvent(e) {
            $('#myModalLabel').text('Edit Event');
            $('#trID').prop('hidden', false);
            $('#tbID').val(e.Id);
            var from = new Date(parseInt(e.Start.substr(6)));
            var to = new Date(parseInt(e.End.substr(6)));
            $('#tbFrmDate').val(formatDate(from));
            $('#tbFrmTime').val(formatTime(from));
            $('#tbTitle').val(e.Title);
            $('#tbToDate').val(formatDate(to));
            $('#tbToTime').val(formatTime(to));
            $('#tbDelete').show();
            $('#tbSave').val('Update');
            if (e.Allday) {
                $('#cbAllDay').prop('checked', true);
                AllDay();
            }
            else {
                $('#cbAllDay').prop('checked', false);
                AllDay();
            }
            if (e.Type == 'Meeting') {
                $('#rbMee').prop('checked', true);
                showMee();
                $('#tbMeeDesc').val(e.Desc);
                $('#<%=ddlType.ClientID%>').val(e.MType);
                $('#tbSave').val('Update and Continue');
                // disable radio buttons
                $('#rbApp').prop('disabled', 'disabled');
                $('#rbCal').prop('disabled', 'disabled');
                $('#rbMee').prop('disabled', false);
            }
            if (e.Type == 'Appointment') {
                $('#rbApp').prop('checked', true);
                showApp();
                $('#tbAppdesc').val(e.Desc);
                $('#<%=tbAppPerson.ClientID %>').val(e.Invited);
                // disable radio buttons
                $('#rbMee').prop('disabled', 'disabled');
                $('#rbCal').prop('disabled', 'disabled');
                $('#rbApp').prop('disabled', false);

            }
            if (e.Type == 'Calendar') {
                $('#rbCal').prop('checked', true);
                showCal();
                // disable radio buttons
                $('#rbMee').prop('disabled', 'disabled');
                $('#rbApp').prop('disabled', 'disabled');
                $('#rbCal').prop('disabled', false);
            }
            $('#myModal').modal('show');
        }
        function getDate(d) {
            var from = d.split("-");
            return newdate = new Date(from[2], from[0] - 1, from[1]);
        }
        function Delete() {
            var date = getDate($('#tbFrmDate').val());
            if (date < Date.now()) {
                $('#ErrorModal').modal('show');
                return false;
            }
            if (confirm('Are you sure you want to Delete?')) {
                var id = $('#tbID').val();
                $.ajax({
                    type: "POST",
                    url: "PCalender.aspx/Delete",
                    data: '{id:"' + id + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var e = response.d;
                        if (e == 'True') {
                            $('#calendar').fullCalendar('refetchEvents');
                            $('#myModal').modal('hide');
                        }
                        else {
                            alert('Error Deleting the event');
                        }

                    },
                    error: function (xh, status, ll) {
                        alert('Error Deleting the event ' + status);
                    }
                });
            }

        }
    </script>
    <style>
        #calendar
        {
            width: 900px;
            margin: 0 auto;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <hr />
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span10" style="border-right: 1px solid #EDEFF0">
                <div id='calendar'>
                </div>
            </div>
            <div class="span2">
                <h4>
                    View Rules:</h4>
                <hr />
                <br />
                <label class="checkbox inline">
                    <input type="checkbox" runat="server" id="cbApp" value="option1" onchange="filter()"><span
                        class="metro-checkbox">Appointments</span>
                </label>
                <label class="checkbox inline">
                    <input type="checkbox" runat="server" id="cbMeet" value="option2" onchange="filter()"><span
                        class="metro-checkbox">Meetings</span>
                </label>
                <label class="checkbox inline">
                    <input type="checkbox" runat="server" id="cbCal" value="option3" onchange="filter()"><span
                        class="metro-checkbox">Activities</span>
                </label>
                <hr />
            </div>
        </div>
    </div>
    <div id="myModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
        aria-hidden="true">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
            </button>
            <h3 id="myModalLabel">
                New Event</h3>
        </div>
        <div class="modal-body">
            <div id="generalDiv">
                <table>
                    <tr>
                        <td width="100">
                            Date and Time
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right">
                            Start
                        </td>
                        <td>
                            <input id="tbFrmDate" type="text" style="width: 100px" />
                            <input id="tbFrmTime" type="text" style="width: 100px" />
                            <label class="checkbox inline">
                                <input type="checkbox" id="cbAllDay" value="2" onclick="AllDay()"><span style="width: 100px"
                                    class="metro-checkbox">All Day Event</span>
                            </label>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right">
                            End
                        </td>
                        <td>
                            <input id="tbToDate" type="text" style="width: 100px" />
                            <input id="tbToTime" type="text" style="width: 100px" />
                        </td>
                    </tr>
                    <tr id="trID" hidden="hidden">
                        <td width="100">
                            ID
                        </td>
                        <td>
                            <input id="tbID" type="text" disabled="disabled" />
                        </td>
                    </tr>
                    <tr>
                        <td width="100px">
                            Title
                        </td>
                        <td>
                            <input id="tbTitle" type="text" />
                        </td>
                    </tr>
                </table>
                <hr />
                <label class="radio">
                    <input type="radio" name="optionsRadios" id="rbCal" value="0" checked="checked" onclick="showCal()">
                    <span class="metro-radio">Calendar entry</span>
                </label>
                <label class="radio">
                    <input type="radio" name="optionsRadios" id="rbApp" value="1" onclick="showApp()">
                    <span class="metro-radio">Appointment</span>
                </label>
                <label class="radio">
                    <input type="radio" name="optionsRadios" id="rbMee" value="2" onclick="showMee()">
                    <span class="metro-radio">Meeting</span>
                </label>
                <hr />
            </div>
            <div hidden="hidden" id="AppointmentModal">
                <table>
                    <tr>
                        <td width="100px">
                            With
                        </td>
                        <td>
                            <asp:TextBox ID="tbAppPerson" runat="server"></asp:TextBox>
                            <asp:AutoCompleteExtender ID="tbAppPerson_AutoCompleteExtender" runat="server" MinimumPrefixLength="1"
                                ServiceMethod="GetCompletionList" TargetControlID="tbAppPerson" UseContextKey="True">
                            </asp:AutoCompleteExtender>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Describtion
                        </td>
                        <td>
                            <textarea cols="" rows="" id="tbAppdesc"></textarea>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="MeetingModal" hidden="hidden">
                <table>
                    <tr>
                        <td width="100px">
                            Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlType" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Describtion
                        </td>
                        <td>
                            <textarea cols="" rows="" id="tbMeeDesc"></textarea>
                        </td>
                    </tr>
                </table>
            </div>
         
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal">
                Close</button>
            <input type="button" id="tbSave" class="btn btn-primary" onclick="SaveEntry()" value="Save" />
            <input type="button" id="tbDelete" class="btn btn-warning" onclick="Delete()" value="Delete"
                style="display: none;" />
        </div>
    </div>
    <div id="ErrorModal" class="modal warning bg-color-blu hide fade" aria-hidden="true"
        aria-labelledby="myModalLabel" role="dialog" tabindex="-1" style="display: none;">
        <div>
            <p>
                You Can not delete a passed event</p>
        </div>
        <div class="modal-footer">
            <button class="btn btn-large" data-dismiss="modal">
                Close</button>
        </div>
    </div>
    <hr />
</asp:Content>
