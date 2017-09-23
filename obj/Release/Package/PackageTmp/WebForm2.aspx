<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="TheBox.WebForm2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <link rel="stylesheet" type="text/css" href="Styles/fullcalendar.css" />
    <link rel="stylesheet" type="text/css" href="Styles/jquery.contextMenu.css" />
    <script language="javascript" type="text/javascript" src="Scripts/overlib.js"></script>
    <script language="javascript" type="text/javascript" src="Scripts/jquery.contextMenu.js"></script>
    <script type='text/javascript'>
        $(document).ready(function () {
            $('#save_event').click(function (event) {
                ajaxSaveEvent();
            });
            $('#add_event').click(function (event) {
                //clear/reset form elements
                //be careful in a full-blown app as I'm 
                //selecting elements by type and could clear other elements on the page
                $('select').attr('selectedIndex', 0);
                $('[type=text]').val('');
                //set id = 0 so when we update, we'll know the Event is a new record
                $('#hdn_id').val(0);
                //display modal dialog form
                $("#dialog-modal").dialog({
                    height: 300,
                    width: 440,
                    modal: true
                });
            });
            //load and render FullCalendar
            loadCalendarEvents();
        });
        function loadCalendarEvents() {
            /*Load and render FullCalendar
            * FullCalendar v1.5.2 http://arshaw.com/fullcalendar/
            --------------------------------------------------------------*/
            $('#calendar').fullCalendar({
                header: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'month,agendaWeek,agendaDay'
                },
                editable: true,
                events: { url: "Events.aspx", borderColor: "#7C95AD" },
                eventDrop: ajaxUpdateDroppedEvent,
                eventResize: ajaxUpdateResizedEvent,
                loading: function (bool) {
                    //could display a 'loading' message
                },
                eventClick: function (event) {
                    //Tooltips are displayed when left-clicking on a Event
                    ajaxGetTooltipInfo(event.id);
                },
                eventMouseover: function (event, jsEvent, view) {
                    /*Hook up right-click context menu
                    A Beautiful Site (http://abeautifulsite.net/)
                    --------------------------------------------------------------*/

                    //Get the real ID from the id property of the element from FullCalendar
                    //I made a change to fullcalendar.js to add an id property to the element
                    //Line 4646 and 4648
                    var id = this.id;
                    var lastUnderscore = id.lastIndexOf('_');
                    id = id.substring(lastUnderscore + 1);

                    $("#hdn_id").val(id);
                    $("#" + this.id).contextMenu({
                        menu: 'myMenu'
                    },
					function (action, el, pos) {
					    //add different action logic here
					    switch (action) {
					        case "edit":
					            showDialog();
					            break;
					        case "delete":
					            ajaxDeleteEvent();
					            break;
					    }
					});
                },
                eventMouseout: function (event, jsEvent, view) {
                    //this closes the toolip message
                    return nd();
                }
            });
        };

        /* Displays the modal dialog for Add and Edit of Events
        ---------------------------------------------------------------------------------------------------*/
        function showDialog() {
            $("#dialog-modal").dialog({
                height: 300,
                width: 440,
                modal: true
            });
            //Load Event from Database
            ajaxGetEventForEdit($("#hdn_id").val());
        };

        /* Ajax method for retrieving information for a specific Event
        ---------------------------------------------------------------------------------------------------*/
        function ajaxGetTooltipInfo(id) {
            //TODO: Refactor
            $.ajax({
                async: false,
                type: "POST",
                url: "Events.aspx/GetEventData",
                data: "{'id':'" + id + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: displayTooltip,
                error: function (result) {
                    alert("Retrieving tooltip failed: " + result.status + ' ' + result.statusText);
                }
            });
        };

        /* Renders the tooltip information from the ajax method
        ---------------------------------------------------------------------------------------------------*/
        function displayTooltip(result) {
            var start_date = new Date(result.d.StartDateAsString);
            var end_date = new Date(result.d.EndDateAsString);
            var tooltip = "Title: " + result.d.Title;
            tooltip += "<br />Start: " + $.fullCalendar.formatDate(start_date, 'MM/dd/yyyy HH:mm:ss');
            tooltip += "<br />End: " + $.fullCalendar.formatDate(end_date, 'MM/dd/yyyy HH:mm:ss');
            tooltip += (result.d.AllDayEvent == false) ? "<br />All Day Event: No" : "<br />All Day Event: Yes";
            tooltip += "<br />ID: " + result.d.CalendarKey;

            return overlib(tooltip);
        };

        /* Generic Ajax failed message
        ---------------------------------------------------------------------------------------------------*/
        function AjaxFailed(result) {
            alert(result.status + ' ' + result.statusText);
        };

        /* Ajax call to update the Event after a drag & drop operation
        If more form fields are added, need to modify the JSON string
        ---------------------------------------------------------------------------------------------------*/
        function ajaxUpdateDroppedEvent(event, dayDelta, minuteDelta, allDay, revertFunc) {
            if (confirm("Are you sure you want to move this event?")) {
                //TODO: Refactor
                $.ajax({
                    async: false,
                    type: "POST",
                    url: "Events.aspx/UpdateEventData",
                    data: "{'id':'" + event.id + "','title':'','start':'" + $.fullCalendar.formatDate(event.start, 'MM/dd/yyyy HH:mm:ss') + "','end':'" + $.fullCalendar.formatDate(event.end, 'MM/dd/yyyy HH:mm:ss') + "','allDayEvent':'','backgroundColor':''}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {
                        $('#calendar').fullCalendar('refetchEvents');
                    },
                    error: function (result) {
                        alert("Update event failed: " + result.status + ' ' + result.statusText);
                        revertFunc(); //FullCalendar: Moves the Event back to its original state.. Very handy!
                    }
                });
            }
            else {
                revertFunc(); //FullCalendar: Moves the Event back to its original state. Very handy!
            };
        };

        /* Ajax call to update the Event after a Resize operation
        If more form fields are added, need to modify the JSON string
        ---------------------------------------------------------------------------------------------------*/
        function ajaxUpdateResizedEvent(event, dayDelta, minuteDelta, allDay, revertFunc) {
            if (confirm("Are you sure you want to move this event?")) {
                //TODO: Refactor
                $.ajax({
                    async: false,
                    type: "POST",
                    url: "Events.aspx/UpdateEventData",
                    data: "{'id':'" + event.id + "','title':'','start':'" + $.fullCalendar.formatDate(event.start, 'MM/dd/yyyy HH:mm:ss') + "','end':'" + $.fullCalendar.formatDate(event.end, 'MM/dd/yyyy HH:mm:ss') + "','allDayEvent':'','backgroundColor':''}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {
                        $('#calendar').fullCalendar('refetchEvents');
                    },
                    error: function (result) {
                        alert("Update event failed: " + result.status + ' ' + result.statusText);
                        revertFunc(); //FullCalendar: Moves the Event back to its original state.. Very handy!
                    }
                });
            }
            else {
                revertFunc(); //FullCalendar: Moves the Event back to its original state. Very handy!
            };
        };

        /* Ajax method for retrieving Event record from database
        Set form elements with appropriate values
        ---------------------------------------------------------------------------------------------------*/
        function ajaxGetEventForEdit(id) {
            //id of the FullCalendar element has a naming convention.
            //a_id_[value of the id set in the JSON data] In this case, CalendarKey
            //TODO: Refactor
            $.ajax({
                async: false,
                type: "POST",
                url: "Events.aspx/GetEventData",
                data: "{'id':'" + id + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    var dtStart = new Date(result.d.StartDateAsString);
                    var dtEnd = new Date(result.d.EndDateAsString);
                    $('#title').val(result.d.Title);
                    $('#start_date').val($.fullCalendar.formatDate(dtStart, 'MM/dd/yyyy'));
                    $('#start_hour').val($.fullCalendar.formatDate(dtStart, 'HH'));
                    $('#start_minute').val($.fullCalendar.formatDate(dtStart, 'mm'));

                    $('#end_date').val($.fullCalendar.formatDate(dtEnd, 'MM/dd/yyyy'));
                    $('#end_hour').val($.fullCalendar.formatDate(dtEnd, 'HH'));
                    $('#end_minute').val($.fullCalendar.formatDate(dtEnd, 'mm'));
                    $('#background_color').val(result.d.BackgroundColor);
                },
                error: AjaxFailed
            });
        };

        /* Ajax method for persisting Event information to database
        ---------------------------------------------------------------------------------------------------*/
        function ajaxSaveEvent() {
            var jsonData = "{";
            jsonData += "'id':'" + $('#hdn_id').val() + "'";
            jsonData += ",'title':'" + $('#title').val() + "'";
            var start_date = new Date($('#start_date').val() + ' ' + $('#start_hour').val() + ':' + $('#start_minute').val());
            var end_date = new Date($('#end_date').val() + ' ' + $('#end_hour').val() + ':' + $('#end_minute').val());

            jsonData += ",'start':'" + $.fullCalendar.formatDate(start_date, 'MM/dd/yyyy HH:mm:ss') + "'";
            jsonData += ",'end':'" + $.fullCalendar.formatDate(end_date, 'MM/dd/yyyy HH:mm:ss') + "'";
            jsonData += ",'allDayEvent':'" + $('#all_day').val() + "'";
            jsonData += ",'backgroundColor':'" + $('#background_color').val() + "'";
            jsonData += "}";
            $.ajax({
                async: false,
                type: "POST",
                url: "Events.aspx/UpdateEventData",
                data: jsonData,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    $("#dialog-modal").dialog('close');
                    $('#calendar').fullCalendar('refetchEvents');

                },
                error: function (result) {
                    alert("Update event failed: " + result.status + ' ' + result.statusText);
                }
            });
        };

        /* Ajax method for deleting Event record
        ---------------------------------------------------------------------------------------------------*/
        function ajaxDeleteEvent() {
            if (!confirm("Are you sure you want to delete?"))
                return;

            $.ajax({
                async: false,
                type: "POST",
                url: "Events.aspx/DeleteEvent",
                data: "{'id':'" + $('#hdn_id').val() + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    $('#calendar').fullCalendar('refetchEvents');
                },
                error: function (result) {
                    alert("Delete event failed: " + result.status + ' ' + result.statusText);
                }
            });
        };

        /* Function for adding days to Date object. - Not used, but could come in handy.
        ---------------------------------------------------------------------------------------------------*/
        Date.prototype.addDays = function (d) {
            this.setDate(this.getDate() + d);
            return this;
        };

        /* Function for adding minutes to Date object. - Not used, but could come in handy.
        ---------------------------------------------------------------------------------------------------*/
        Date.prototype.addMinutes = function (m) {
            this.setMinutes(this.getMinutes() + m);
            return this;
        };
    </script>    

</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input type="button" id="add_event" value="Add Event" />
    <div id="calendar" style="margin:3em 0;font-size:13px"></div>
    <ul id="myMenu" class="contextMenu">
		<li class="edit"><a href="#edit">Edit</a></li>
		<li class="delete"><a href="#delete">Delete</a></li>
	</ul>
    <ul id="calendar_menu" class="contextMenu">
        <li class="edit"><a href="#edit">New Event</a></li>
    </ul>
<div class="ui-dialog ui-widget ui-widget-content ui-corner-all ui-draggable ui-resizable" id="dialog-modal" style="display:none;">
   <div id="dialog-form">
	<p class="validateTips">All form fields are required.</p>
	    <fieldset>
            <table>
                <tr>
                    <td>Title: <input type="text" id="title" class="text ui-widget-content ui-corner-all" /></td>
                </tr>
                <tr>
                    <td>
                        <table width="100%">
                        <tr>
                            <td style="width:75px;">Start Date:</td>
                            <td><input type="text" name="start_date" id="start_date" style="width:100px;" class="text ui-widget-content ui-corner-all" /></td>
                            <td><select id="start_hour" class="text ui-widget-content ui-corner-all">
                                <option value="00">00</option>
                                <option value="01">01</option>
                                <option value="02">02</option>
                                <option value="03">03</option>
                                <option value="04">04</option>
                                <option value="05">05</option>
                                <option value="06">06</option>
                                <option value="07">07</option>
                                <option value="08">08</option>
                                <option value="09">09</option>
                                <option value="10">10</option>
                                <option value="11">11</option>
                                <option value="12">12</option>
                                <option value="13">13</option>
                                <option value="14">14</option>
                                <option value="15">15</option>
                                <option value="16">16</option>
                                <option value="17">17</option>
                                <option value="18">18</option>
                                <option value="19">19</option>
                                <option value="20">20</option>
                                <option value="21">22</option>
                                <option value="23">23</option>
                            </select>:<select id="start_minute" class="text ui-widget-content ui-corner-all">
                                <option value="00">00</option>
                                <option value="15">15</option>
                                <option value="30">30</option>
                                <option value="45">45</option>
                            </select></td>
                        </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table width="100%">
                        <tr>
                            <td style="width:75px;">End Date:</td>
                            <td><input type="text" name="end_date" id="end_date" style="width:100px;" class="text ui-widget-content ui-corner-all" /></td>
                            <td><select id="end_hour" class="text ui-widget-content ui-corner-all">
                                <option value="00">00</option>
                                <option value="01">01</option>
                                <option value="02">02</option>
                                <option value="03">03</option>
                                <option value="04">04</option>
                                <option value="05">05</option>
                                <option value="06">06</option>
                                <option value="07">07</option>
                                <option value="08">08</option>
                                <option value="09">09</option>
                                <option value="10">10</option>
                                <option value="11">11</option>
                                <option value="12">12</option>
                                <option value="13">13</option>
                                <option value="14">14</option>
                                <option value="15">15</option>
                                <option value="16">16</option>
                                <option value="17">17</option>
                                <option value="18">18</option>
                                <option value="19">19</option>
                                <option value="20">20</option>
                                <option value="21">22</option>
                                <option value="23">23</option>
                            </select>:<select id="end_minute" class="text ui-widget-content ui-corner-all">
                                <option value="00">00</option>
                                <option value="15">15</option>
                                <option value="30">30</option>
                                <option value="45">45</option>
                            </select></td>
                        </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>All Day Event: <select id="all_day" class="text ui-widget-content ui-corner-all">
                        <option value="false">No</option>
                        <option value="true">Yes</option>
                    </select></td>
                </tr>
                <tr>
                    <td>Background Color: <input type="text" id="background_color" class="text ui-widget-content ui-corner-all" /></td>
                </tr>
                <tr>
                    <td style="text-align:center"><input type="button" id="save_event" value="Save" /></td>
                </tr>
            </table>
            <input type="hidden" id="hdn_id" value="" />
	    </fieldset>
</div>
</div>
    </div>
    </form>
</body>
</html>
