<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="TheBox.Protected._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h4>
        you have 3 messages and 2 notifications
    </h4>
     <div class="container-fluid">
<div class="row-fluid">
<div class="span10">
    <asp:GridView ID="gvFiles" runat="server" AutoGenerateColumns="False" 
        Width="100%">
        <Columns>
            <asp:BoundField DataField="FileName" HeaderText="File Name" 
                SortExpression="FileName" />
            <asp:BoundField DataField="Size" HeaderText="Size" SortExpression="Size" />
        </Columns>
    </asp:GridView>
</div>
<div class="span2">
Collaborate
Document Management
<!--Sidebar content-->
</div>

</div>
</div>
   
</asp:Content>
