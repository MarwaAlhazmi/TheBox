<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditUser.aspx.cs" Inherits="TheBox.Account.EditUser" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager2" runat="server">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <h2>Edit User Information</h2>
 <hr />
  
  <div>
     <table style="width: 100%;">
         <tr>
             <td style="width:150px">
                 User Name</td>
             <td>
                 <asp:TextBox ID="tbuname" runat="server" ReadOnly="True"></asp:TextBox>
             </td>
             <td>
                 &nbsp;
             </td>
         </tr>
         <tr>
             <td>
                 First Name</td>
             <td>
                 <asp:TextBox ID="tbfname" runat="server"></asp:TextBox>
             </td>
             <td>
                 &nbsp;
             </td>
         </tr>
         <tr>
             <td>
                 Last Name</td>
             <td>
                 <asp:TextBox ID="tblname" runat="server"></asp:TextBox>
             </td>
             <td>
                 &nbsp;
             </td>
         </tr>
         <tr>
             <td>
                 Email</td>
             <td>
                 <asp:TextBox ID="tbemail" runat="server"></asp:TextBox>
             </td>
             <td>
                 &nbsp;</td>
         </tr>
         <tr>
             <td>
                 Position</td>
             <td>
                 <asp:DropDownList ID="ddlPosition" runat="server">
                 </asp:DropDownList>
             </td>
             <td>
                 &nbsp;</td>
         </tr>
         <tr>
             <td>
                 
             </td>
             <td>
                 <asp:Button ID="btnPass" runat="server" Text="Reset Password" />
             </td>
             <td>
                 &nbsp;</td>
         </tr>
     </table>
     <hr />
     <asp:Button ID="btnSave" runat="server" Text="Save" onclick="btnSave_Click" />
 </div>
 </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
