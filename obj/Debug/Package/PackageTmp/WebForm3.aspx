<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="WebForm3.aspx.cs" Inherits="TheBox.WebForm3" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <div class="taskBox">
    <div class="span1 pull-right" style="border: 1px solid Green; background-color: #66C266;
                width: 100px; color: White; text-align: center">
                Completed</div>
        <div class="row-fluid">
            <div class="span4">
                <strong>Action </strong><span style="color: Red">lkasjg;jhg</span>
            </div>
            <div class="span3">
                <strong>created: </strong>  12/12/2013
            </div>
            <div class="span3" style="color:Red">
                <strong>date : </strong>  12/12/2013
            </div>
        </div>
        <div class="row-fluid">
        <div class="span4">
        <strong>Describtion: </strong> kljasfjksdflsdhgfskdjgfskdaj
        </div>
        <div class="span7">
        <strong>By Users: </strong> sfsd; sdgfsdf; sdfsdfg; ;sdsdfsd;
        </div>
        </div>
    </div>
    <div class="taskBox">
        <table style="width: 100%">
            <tr>
                <td style="width: 100px">
                    Action:
                </td>
                <td>
                    l;kjfsdkljhglskjghslkdjfg
                </td>
                <td style="width: 100px">
                    Date Created
                </td>
                <td style="padding: 0px 10px 0px 5px">
                    12/12/2013
                </td>
                <td style="width: 100px">
                    Due Date
                </td>
                <td style="color: Red">
                    12/12/2013
                </td>
                <td class="pull-right" id="CompleteTD" style="border: 1px solid Green; background-color: #66C266;
                    width: 100px; color: White">
                    Completed
                </td>
            </tr>
            <tr>
                <td style="width: 100px">
                    Describtion
                </td>
                <td style="width: 200px" colspan="3">
                    l;kjfsdkljhglskjghslkdjfg sd;kljslkd alsdkhfjskdf lkahsd;kjgfsa lksjhdkfa
                </td>
                <td>
                    By users
                </td>
                <td>
                    dsfsdf; fsdfs; fsdfs
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
        </table>
        <hr />
        <div>
            <div>
                <strong>Ahmad</strong> 11/27/2013
                <div class="pull-right">
                    <span style="color: Green"><b>Complete</b></span>
                </div>
            </div>
        </div>
        <hr />
        <div>
            <div>
                <b>Ahmad</b> 11/27/2013
                <div class="pull-right">
                    <span style="color: Green"><b>Complete</b></span>
                </div>
            </div>
        </div>
    </div>
    <hr />
    <div class="taskBox">
        <table style="width: 100%">
            <tr>
                <td style="width: 80px">
                    Action:
                </td>
                <td>
                    l;kjfsdkljhglskjghslkdjfg
                </td>
                <td style="width: 100px">
                    Date Created
                </td>
                <td style="padding: 0px 10px 0px 5px">
                    12/12/2013
                </td>
                <td style="width: 100px">
                    Due Date
                </td>
                <td>
                    <div style="width: 80%; color: red">
                        10/31/2013
                    </div>
                </td>
                <td class="pull-right" id="Td1" style="border: 1px solid red; background-color: #FF4D4D;
                    color: White">
                    Decline Task
                </td>
            </tr>
        </table>
    </div>
    <hr />
    <div class="taskBox">
        <div class="row-fluid">
            <table style="width: 100%">
                <tr>
                    <td style="width: 80px">
                        Action:
                    </td>
                    <td>
                        l;kjfsdkljhglskjghslkdjfg
                    </td>
                    <td style="width: 100px">
                        Date Created
                    </td>
                    <td style="padding: 0px 10px 0px 5px">
                        12/12/2013
                    </td>
                    <td style="width: 100px">
                        Due Date
                    </td>
                    <td>
                        <div style="width: 80%">
                            10/31/2013
                        </div>
                    </td>
                    <td id="Td2" style="border: 1px solid Blue; background-color: #4D70DB; color: White;
                        width: 200px; text-align: center">
                        Comment
                    </td>
                </tr>
            </table>
        </div>
        <hr />
        <div class="row-fluid">
            <div>
                <b>Ahmad</b> 11/27/2013
                <div class="pull-right">
                    <span style="color: Green"><b>Complete</b></span>
                </div>
            </div>
        </div>
        <hr />
        <div>
            <div>
                <b>Ahmad</b> 11/27/2013
                <div class="pull-right">
                    <span style="color: Green"><b>Complete</b></span>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
