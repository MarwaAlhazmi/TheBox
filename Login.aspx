<%@ Page Title="Log In" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="TheBox.Login" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="container-fluid">
    <br />
        <div class="row-fluid">
        <div class="span7">
        <div>
        <br /><br /><br />

        <div class="carousel slide" id="myCarousel">
                   <div class="carousel-inner">
                     <div class="item active">
                       <img alt="" src="content/img/bootstrap-mdo-sfmoma-01.jpg">
                       <div class="carousel-caption">
                         <h4>First Thumbnail label</h4>
                         <p>Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.</p>
                       </div>
                     </div>
                     <div class="item">
                       <img alt="" src="content/img/bootstrap-mdo-sfmoma-02.jpg">
                       <div class="carousel-caption">
                         <h4>Second Thumbnail label</h4>
                         <p>Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.</p>
                       </div>
                     </div>
                     <div class="item">
                       <img alt="" src="content/img/bootstrap-mdo-sfmoma-03.jpg">
                       <div class="carousel-caption">
                         <h4>Third Thumbnail label</h4>
                         <p>Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.</p>
                       </div>
                     </div>
                   </div>
                   <a data-slide="prev" href="#myCarousel" class="left carousel-control"><span class="icon-arrow-left"></span></a>
                   <a data-slide="next" href="#myCarousel" class="right carousel-control"><span class="icon-arrow-right"></span></a>
                 </div>
        </div>
        </div>
        <div class="span5">
        <br /><br /><br />
           <h2>
        Log In
    </h2>
   <br />
    <asp:Login ID="LoginUser" runat="server" EnableViewState="false" RenderOuterTable="false"
        DestinationPageUrl="~/Default.aspx">
        <LayoutTemplate>
            <span class="failureNotification">
                <asp:Literal ID="FailureText" runat="server"></asp:Literal>
            </span>
            <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification"
                ValidationGroup="LoginUserValidationGroup" />
            <div class="accountInfo">
                <fieldset class="login">
                    <legend>Account Information</legend>
                    <p>
                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">Username:</asp:Label>
                        <asp:TextBox ID="UserName" runat="server" CssClass="textEntry"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                            CssClass="failureNotification" ErrorMessage="User Name is required." ToolTip="User Name is required."
                            ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                    </p>
                    <p>
                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                        <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                            CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Password is required."
                            ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                    </p>
                    <p>
                        <asp:CheckBox ID="RememberMe" runat="server" CssClass="inline" />
                        <asp:Label ID="RememberMeLabel" runat="server" AssociatedControlID="RememberMe" CssClass="inline">Keep me logged in</asp:Label>
                    </p>
                    <br />
                    <p class="submitButton">
                    <asp:Button ID="LoginButton" CssClass="pull-right" runat="server" CommandName="Login" Text="Log In" ValidationGroup="LoginUserValidationGroup" />
                </p>
                </fieldset>
                
            </div>
        </LayoutTemplate>
    </asp:Login>
        </div>
        </div>
    </div>
 
</asp:Content>
