<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageUser.aspx.cs" Inherits="TheBox.Account.ManageUser" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="../Scripts/bootstrap.js" type="text/javascript"></script>
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <h2>Manage Users Accounts</h2>
    <hr />
           <asp:Button ID="btnAdd" runat="server" Text="Add New User" 
            CssClass="pull-right" onclick="btnAdd_Click" />
            <br />
            <br />
        <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" 
            CssClass="table table-striped" Width="100%" onrowediting="gvUsers_RowEditing" 
            onrowdeleting="gvUsers_RowDeleting" AllowSorting="True">
            <EmptyDataTemplate>No Data Available</EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="UserName" HeaderText="User Name" 
                    SortExpression="UserName" />
                <asp:BoundField DataField="firstName" HeaderText="First Name" 
                    SortExpression="firstName" />
                <asp:BoundField DataField="lastName" HeaderText="Last Name" 
                    SortExpression="lastName" />
                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                <asp:CommandField ShowEditButton="True">
                <ItemStyle Width="60px" />
                </asp:CommandField>
                <asp:CommandField ShowDeleteButton="True">
                <ItemStyle Width="60px" />
                </asp:CommandField>
            </Columns>
        </asp:GridView>
        
     

             <div id="myModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
<div class="modal-header">
<button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
<h3 id="myModalLabel">Modal header</h3>
</div>
<div class="modal-body">
<p>One fine body…</p>
</div>
<div class="modal-footer">
<button class="btn" data-dismiss="modal" aria-hidden="true">Close</button>
<button class="btn btn-primary">Save changes</button>
</div>
</div>
    </ContentTemplate>
    </asp:UpdatePanel>
    
</asp:Content>
