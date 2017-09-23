<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" Async="true"
    CodeBehind="Update.aspx.cs" Inherits="TheBox.Update" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<script type="text/javascript">
    function filter(obj) {
        $('#btnText').html(obj + '<span class="caret"></span>');
        // filter based on title= obj

        $('#<%=updates.ClientID%> > div').each(function (index, div) {
            $d = $(div);
            var a = $d.find('a:first-child');
            var title = a.prop('title');
            if (obj == 'All') {
                $d.show('fast');
            }
            else {
                if (title != obj) {
                    $d.hide('fast');
                }
                else {
                    $d.show('fast');
                }
            }
        });
        return false;
    }

    function clickLink(link, id) {
        // first
        $.ajax({
            type: "POST",
            url: "Update.aspx/UpdateNotification",
            data: '{NotiID:"' + id + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                window.location = link;
            },
            error: function () {
                PError('Error fetching agenda privacy information, Try again!');
            }
        });
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel runat="server" ID="updatepanel1" UpdateMode="Conditional">
        <ContentTemplate>
            <h2>
                Updates</h2>
            <hr />
            <div class="row-fluid">
                <div class="span2">
                    <h4>
                        Recent Updates:
                    </h4>
                </div>
                <div class="span2">
                    <div class="btn-group">
                        <a class="btn dropdown-toggle" id="btnText" data-toggle="dropdown" href="#">All <span class="caret">
                        </span></a>
                        <ul class="dropdown-menu">
                            <li><a href="#" onclick="return filter('Meetings')">Meetings</a></li>
                            <li><a href="#" onclick="return filter('Projects')">Projects</a></li>
                            <li><a href="#" onclick="return filter('Agenda')">Agenda</a></li>
                            <li><a href="#" onclick="return filter('Discussions')">Discussions</a></li>
                            <li><a class="divider"></a></li>
                            <li><a href="#" onclick="return filter('All')">All</a></li>
                        </ul>
                    </div>
                </div>
                <br /><hr />
            </div>
            <div class="row-fluid">
           
                <div id="test" runat="server">
                
                </div>

                <div id="updates" runat="server" class="span8" style=" padding-left:5px">
                <br />
                
                </div>
                <br />
                <div id="cre" runat="server">
                
                </div>
                
            </div>


        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
