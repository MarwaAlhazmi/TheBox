<%@ Page Title="About Us" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="TheBox.About" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#btnSave").click(function (event) {
                event.preventDefault();
                var name1 = $('#Text1').val();
                $.ajax({
                    type: "POST",
                    url: "About.aspx/GetTeams",
                    data: '{name: "' + name1 + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $('#result').html(response.d);
                        $('#myModal').modal('hide');
                    }
                });
            });
        })

//        $(function () {
//            $('#btnTest').click(function () {
//                var $ = jQuery.noConflict();
//                //var name1 = $('#Text1').val();
//                $.ajax({
//                    type: "POST",
//                    url: "About.aspx/GetTeams",
//                    data: '{name: "marwa"}', //name:"' + name1 + '"
//                    contentType: "application/json; charset=utf-8",
//                    dataType: "json",
//                    success: function (response) {
//                        $('#ptest').html(response.d);
//                    }
//                }); (jQuery);
//            });
//        });
//	
   
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel runat="server" ID="updatepanel1">
    <ContentTemplate>
    
    
    <h1>
        Football Teams</h1>
    <button type="button" id="buttonTeam" runat="server">
        Get Teams</button>
    <div id="result">
        hello</div>
    <a href="#myModal" role="button" class="btn" data-toggle="modal">Launch demo modal</a>
    <!-- Modal -->
    <div id="myModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
        aria-hidden="true">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
            </button>
            <h3 id="myModalLabel">
                Modal header</h3>
        </div>
        <div class="modal-body">
            <p id="ptest">
                One fine body…</p>
            <input id="Text1" type="text" />
            <button id="btnTest">
                Test</button>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal" aria-hidden="true">
                Close</button>
            <button id="btnSave" class="btn btn-primary">
                Save changes</button>
        </div>
    </div>

    </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
