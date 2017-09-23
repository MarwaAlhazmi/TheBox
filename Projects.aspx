<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Projects.aspx.cs" Inherits="TheBox.Projects" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="content/ico/favicon.ico" rel="shortcut icon">
    <link href="content/ico/apple-touch-icon-144-precomposed.png" sizes="144x144" rel="apple-touch-icon-precomposed">
    <link href="content/ico/apple-touch-icon-114-precomposed.png" sizes="114x114" rel="apple-touch-icon-precomposed">
    <link href="content/ico/apple-touch-icon-72-precomposed.png" sizes="72x72" rel="apple-touch-icon-precomposed">
    <link href="content/ico/apple-touch-icon-57-precomposed.png" rel="apple-touch-icon-precomposed">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#btnSave").click(function (event) {
                var $ = jQuery.noConflict();
                event.preventDefault();
                var folder = $('#tbFolder').val();
                $.ajax({
                    type: "POST",
                    url: "Projects.aspx/createFolder",
                    data: '{name: "' + folder + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $('#divtest').html(response.d);
                        $('#myModal').modal('hide');
                    }
                });
            });
        })

       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" EnablePageMethods="True">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <h2>
                Projects</h2>
            <hr />
            <div class="container-fluid">
                <div class="row-fluid">
                    <div class="span4" style="margin-right: 20px">
                        <asp:TabContainer ID="TabContainer1" runat="server" Width="100%" ScrollBars="Auto"
                            Height="700px">
                            <asp:TabPanel runat="server" ID="panel1" HeaderText="Titles" ScrollBars="Horizontal">
                                <ContentTemplate>
                                    <asp:TreeView ID="TreeView1" runat="server" ImageSet="Arrows" ExpandDepth="1" OnSelectedNodeChanged="TreeView1_SelectedNodeChanged">
                                        <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                                        <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" HorizontalPadding="5px"
                                            NodeSpacing="0px" VerticalPadding="0px" />
                                        <ParentNodeStyle Font-Bold="False" />
                                        <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px"
                                            VerticalPadding="0px" /><Nodes><asp:TreeNode></asp:TreeNode></Nodes>
                                    </asp:TreeView>
                                </ContentTemplate>
                            </asp:TabPanel>
                            <asp:TabPanel runat="server" ID="panel2" HeaderText="Folder">
                                <ContentTemplate>
                                    <asp:TreeView ID="tvFiles" runat="server" ImageSet="Arrows" ExpandDepth="1" OnSelectedNodeChanged="tvFiles_SelectedNodeChanged">
                                        <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                                        <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" HorizontalPadding="5px"
                                            NodeSpacing="0px" VerticalPadding="0px" />
                                        <ParentNodeStyle Font-Bold="False" />
                                        <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px"
                                            VerticalPadding="0px" />
                                    </asp:TreeView>
                                </ContentTemplate>
                            </asp:TabPanel>
                            <asp:TabPanel runat="server" ID="search" HeaderText="Search">
                                <ContentTemplate>
                                    <table>
                                <tr>
                                    <td width="200px">
                                        Title :
                                    </td>
                                    <td>
                                        <input type="text" runat="server" id="tbtitle" value="" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Date Start :
                                    </td>
                                    <td>
                                        <input type="text" runat="server" id="tbDateS" class="datepicker"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Date End :
                                    </td>
                                    <td>
                                        <input id="tbDateE" type="text" runat="server" class="datepicker"/>
                                    </td>
                                </tr>
                        </table><div align="right">
                        <button id="btnSearch" runat="server" class="btn btn-primary icon-search-2">Search</button>
                        <button id="btnCancel" runat="server" class="btn-danger">Cancel</button>  </div>
                                </ContentTemplate>
                            </asp:TabPanel>
                        </asp:TabContainer>
                    </div>
                    <div class="span6" style="margin-left: 20px; margin-right: 20px; border-right: 3px solid Gray">
                        <div class="row">
                                <a href="#" class="  pull-right btn-link icon-arrow-down-right-2">Follow</a>
                                <a href="#folderModal" class=" pull-right btn-link icon-folder-2" data-toggle="modal">Create New Folder</a><br />
                                
                            <!-- Modal -->
                            <div id="folderModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
                                aria-hidden="true">
                                <div class="modal-header">
                                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                                    </button>
                                    <h3 id="H1">
                                        Create New Folder</h3>
                                </div>
                                <div class="modal-body">
                                    <!-- new panel -->
                                    <b>Folder Name: </b>
                                    <input id="tbFolder" type="text" />
                                </div>
                                <div class="modal-footer">
                                    <button class="btn" data-dismiss="modal" aria-hidden="true">
                                        Close</button>
                                    <button class="btn btn-primary" id="btnSave" runat="server">
                                        Save changes</button>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <iframe src="" runat="server" id="docPreview" style="width: 100%; height: 700px"
                                frameborder="0"></iframe>
                        </div>
                        <div class="row" id="testdiv">
                        </div>
                        <div class="row" runat="server" id="commentsContainer" visible="false" style="background-color: #eee">
                            <div class="span8">
                                <!--<a class="btn" href="#" style="margin-left: 10px; margin-top: 10px">Like </a>-->
                                <a href="#myModal" role="button" class="btn" data-toggle="modal" style="margin-top: 10px">
                                    Assign task</a>
                                <!-- Modal -->
                                <div id="myModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
                                    aria-hidden="true">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                                        </button>
                                        <h3 id="myModalLabel">
                                            Assign task</h3>
                                    </div>
                                    <div class="modal-body">
                                        <!-- new panel -->
                                        <table>
                                            <tr>
                                                <td>
                                                    Due Date:
                                                </td>
                                                <td>
                                                    <input type="text" id="dp1" value="02-16-2012" class="span4 datepicker">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Task
                                                </td>
                                                <td>
                                                    <div class="btn-group" style="width: 60px">
                                                        <a class="btn dropdown-toggle" data-toggle="dropdown" href="#">To Review <span class="caret">
                                                        </span></a>
                                                        <ul class="dropdown-menu">
                                                            <li><a tabindex="-1" href="#">To Approve</a></li>
                                                            <li><a tabindex="-1" href="#">To Update</a></li>
                                                        </ul>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    User
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddlUsers" runat="server">
                                                        <asp:ListItem>Marwa</asp:ListItem>
                                                        <asp:ListItem>Ahmad</asp:ListItem>
                                                        <asp:ListItem>Siti</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div class="modal-footer">
                                        <button class="btn" data-dismiss="modal" aria-hidden="true">
                                            Close</button>
                                        <button class="btn btn-primary">
                                            Save changes</button>
                                    </div>
                                </div>
                                <hr />
                                <div class="tile-listviewitem-container" id="tilelistviewdemo" runat="server" style="margin-left: 20px">
                                </div>
                                <textarea id="tbComment" runat="server" rows="0" cols="20" class="input-block-level"
                                    type="text" placeholder="Insert a comment" style="margin-left: 10px"></textarea>
                                <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn" OnClick="btnSubmit_Click" />
                            </div>
                            <!-- comment panel-->
                        </div>
                    </div>
                    <div class="offset10" style="margin-left: 20px" />
                    <h4>
                        File Info</h4>
                    <hr />
                    <div runat="server" id="settingsDiv">
                        <h5 runat="server" id="filename">
                            <b>File name: </b>
                        </h5>
                        <h5 runat="server" id="size">
                            <b>Size: </b>
                        </h5>
                        <h5 runat="server" id="creator">
                            <b>Creator: </b>
                        </h5>
                    </div>
                    <br />
                    <h4>
                        Users</h4>
                    <hr />
                    <div>
                        <table>
                            <tr>
                                <td>
                                    <h5>
                                        <a href="#">Marwa</a></h5>
                                </td>
                                <td>
                                    <div class="btn-group" style="width: 60px">
                                        <a class="btn dropdown-toggle" data-toggle="dropdown" href="#">Creator <span class="caret">
                                        </span></a>
                                        <ul class="dropdown-menu">
                                            <li><a tabindex="-1" href="#">Editor</a></li>
                                            <li><a tabindex="-1" href="#">Viewer</a></li>
                                        </ul>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <h5>
                                        <a href="#">Ahmed</a></h5>
                                </td>
                                <td>
                                    <div class="btn-group">
                                        <a class="btn dropdown-toggle" data-toggle="dropdown" href="#" style="width: 60px">Editor
                                            <span class="caret"></span></a>
                                        <ul class="dropdown-menu">
                                            <li><a tabindex="-1" href="#">Creator</a></li>
                                            <li><a tabindex="-1" href="#">Viewer</a></li>
                                        </ul>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <br />
                    <h4>
                        Actions</h4>
                    <hr />
                    <div>
                        <a href=""><i class="icon-download"></i>Download</a><br />
                        <a href="#"><i class="icon-upload"></i>Uplaod</a><br />
                        <a href="#"><i class="icon-share"></i>Share</a>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
