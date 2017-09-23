<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Agenda.aspx.cs" Inherits="TheBox.AgendaP" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script>
        function test() {
            $.ajax({
                type: "POST",
                url: "Agenda.aspx/getAgenda",
                data: '{}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    if (result.hasOwnProperty("d"))
                    { result = result.d; }
                    for (var i = 0; i < 2; i++) {
                        $('#test').append("<br />" + result[i].test1);
                    }
                },
                error: function () { alert("error"); }

            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Agenda</h2>
    <hr />
    <div style="margin-bottom; position: absolute; z-index: auto; width: auto; height: auto;
        top: auto; right: auto; bottom: auto; left: auto;">
        <table class="table-bordered" id="tblAgenda" runat="server">
            <tr>
                <td align="center" style="font-weight: bold">
                    ID
                </td>
                <td align="center" style="font-weight: bold">
                    Title
                </td>
                <td align="center" style="font-weight: bold">
                    Description
                </td>
                <td align="center" style="font-weight: bold">
                    Action
                </td>
                <td align="center" style="font-weight: bold">
                    &nbsp;
                </td>
            </tr>
        </table>
        <br />
        <button id="add" class="btn btn-success" data-target="#myModal1" data-toggle="modal">
            Add Agenda</button>&nbsp&nbsp
    </div>
   
    <div id="myModal1" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
        aria-hidden="true">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
            </button>
            <h3 id="myModalLabel">
                Add Agenda</h3>
        </div>
        <div class="modal-body">
            <div class="offset1">
                <table>
                    <tr>
                        <td width="200px">
                            ID
                        </td>
                        <td>
                            <input type="text" runat="server" id="id" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Title
                        </td>
                        <td>
                            <input type="text" runat="server" id="title" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Description
                        </td>
                        <td>
                            <textarea rows="5" cols="1" id="description" runat="server"></textarea>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Action Required
                        </td>
                        <td>
                            <button id="note" class="btn" data-target="" data-toggle="" style="width: 130px">
                                Noted</button><br />
                            <button id="approval" class="btn" data-target="" data-toggle="" style="width: 130px">
                                Get Approval</button><br />
                            <button id="action" type="button" class="btn" data-toggle="collapse" data-target="#demo"
                                style="width: 130px">
                                Action</button><br />
                            <br />
                            <div id="demo" class="collapse open">
                                <table>
                                    <tr>
                                        <td width="100px">
                                            Assignment
                                        </td>
                                        <td>
                                            <textarea rows="3" cols="1" id="task" runat="server"></textarea>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="modal-footer">
                <button id="btnSave" class="btn btn-info" runat="server" onclick="">
                    Save</button>
                <button id="btnPublish" class="btn btn-primary" runat="server" onclick="">
                    Publish</button>
                <button id="btnCancel" class="btn btn-warning" data-dismiss="modal" aria-hidden="true">
                    Cancel</button>
            </div>
        </div>
    </div>
</asp:Content>
