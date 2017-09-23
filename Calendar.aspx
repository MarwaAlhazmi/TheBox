<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Calendar.aspx.cs" Inherits="TheBox.Calendar" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="content/css/style10/style.css" rel="stylesheet" type="text/css" />
    <script>
        function myFunction(obj) {

            $('#myModal').modal('show');
            var month = $(obj).attr("id");
            $('#tbDate').val(month);
            return false;
        }

        function redirect() {
            var title = $('#tbTitle').val();
            var date = $('#tbDate').val();
            var url = "PMeetingSetUp.aspx?date=" + date + "&title=" + title;
            window.location = url;
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="updatepanle1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    

    <div class="container">
        <div class="row-fluid">
            <div class="span10 offset1">
                <asp:Calendar ID="calTraining" runat="server" CssClass="table-cell-hover"
                    DayNameFormat="Full"
                    Height="500px" Width="100%" ShowNextPrevMonth="true"
                    OnDayRender="calTraining_DayRender" FirstDayOfWeek="Monday">
                    <TitleStyle BackColor="White" BorderColor="ActiveCaptionText" HorizontalAlign="Center" Font-Bold="True" Font-Size="14pt" />
                    <DayHeaderStyle BackColor="Silver" BorderColor="Silver" HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="#2685EE"/>
                </asp:Calendar>
            </div>
        </div>
    </div>
    
    <!-- Modal -->
    <div id="myModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
        aria-hidden="true">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
            </button>
            <h3 id="myModalLabel">
                Set Up a New Meeting</h3>
        </div>
        <div class="modal-body">
            <p>Meeting Title: </p> 
            <input id="tbTitle" type="text" />
            <input type="hidden" id="tbDate" type="text" value="hello there"/>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal" aria-hidden="true">
                Close</button>
            <button id="btnok" class="btn btn-primary" onclick="return redirect()">
                OK</button>
        </div>
    </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
