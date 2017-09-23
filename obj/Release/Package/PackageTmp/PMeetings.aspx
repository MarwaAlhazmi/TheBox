<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="PMeetings.aspx.cs" Inherits="TheBox.PMeetings" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script>
        function test(event) {
            $('#progressDiv').show();
            var hh = $(event).attr('href');
            var id = hh.substring(1, hh.length);
            $.ajax({
                type: "POST",
                url: "PMeetings.aspx/getInfo",
                data: '{href:"' + id + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    $('#progressDiv').hide();
                    $('#' + id.toString()).html(response.d);
                }

            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel runat="server" ID="updatepanle1">
        <ContentTemplate>
            <ul class="nav nav-tabs" id="myTab">
                <li class="active"><a href="#coming" data-toggle="tab">Upcoming Meetings</a></li>
                <li><a href="#month" data-toggle="tab">This Month's Meetings</a></li>
                <li><a href="#history" data-toggle="tab">History</a></li>
            </ul>
            <div class="tab-content">
                <div class="tab-pane active" id="coming">
                    <div class="container-fluid">
                        <div class="row-fluid">
                            <div class="span10" style="border-right: 1px solid Gray; overflow-x: hidden; overflow: scroll;
                                height: 400px">
                                <!--Body content-->
                                <div class="row-fluid">
                                    <div class="accordion" id="accordion2" runat="server">
                                    </div>
                                </div>
                                <hr />
                            </div>
                            <div class="span2">
                                <!--Side bar content-->
                                <a href="PMeetingSetUp.aspx"><i class="icon-download"></i>Set up a Meeting</a><br />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="tab-pane" id="month">
                    <div class="span10" style="border-right: 1px solid Gray; overflow-x: hidden; overflow: scroll;
                        height: 400px">
                        <div class="accordion" id="accordionMonth" runat="server">
                        </div>
                    </div>
                </div>
                <div class="tab-pane" id="history">
                    <div class="row-fluid">
                        <div class="span10" style="border-right: 1px solid Gray; overflow-x: hidden; overflow: scroll;
                            height: 400px">
                            <div class="accordion" id="accordionHistory" runat="server">
                            </div>
                        </div>
                        <div class="span1">
                            <div class="progress progress-indeterminate" id="progressDiv" hidden="hidden" style="display: none;">
                                <div class="win-ring small">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
