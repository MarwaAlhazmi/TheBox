<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
CodeBehind="~/PMeetingDetail.aspx.cs" Inherits="TheBox.PMettingDetails" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="https://apis.google.com/js/plusone.js"></script>
    <script src="content/js/datepair.js" type="text/javascript"></script>
    <script>
    function getInfo() {

        $('#<%=tbtitle.ClientID%>').val($('#<%=tbtitle2.ClientID%>').val());
        $('#<%=tbdate.ClientID%>').val($('#<%=tbdate2.ClientID%>').val());
        $('#<%=tbSTime2.ClientID%>').val($('#<%=tbSTime.ClientID%>').val());
        $('#<%=tbETime2.ClientID%>').val($('#<%=tbETime.ClientID%>').val());
        $('#<%=tbIndo.ClientID%>').val($('#<%=tbIndo2.ClientID%>').val());

        }

        function send() {
            var urlParams;
            (window.onpopstate = function () {
                var match,
                pl = /\+/g,  // Regex for replacing addition symbol with a space
                search = /([^&=]+)=?([^&]*)/g,
                decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
                query = window.location.search.substring(1);

                urlParams = {};
                while (match = search.exec(query))
                    urlParams[decode(match[1])] = decode(match[2]);
            })();
            var id = urlParams["id"];

            $.ajax({
                type: "POST",
                url: "PMeetingDetail.aspx/updateInfo",
                data: '{}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {

                    $('#<%=tbIndo2.ClientID%>').val(result.d);
                    $('#myModal1').modal('hide');
                },
                error: function () { alert("Got Error!!"); }
            });
        }

        function addItem() {
            var item = $('#<%=ddlItems.ClientID %> option:selected').text();
            var itemID = $('#<%=ddlItems.ClientID%>').val();
            var qnt = $('#<%=tbQuantity.ClientID %>').val();
            var str = ' <div class="alert" style="background-color:Gray; border-color:Black">' + qnt + ' | ' + item + '<button class="close" data-dismiss="alert" type="button"></button></div>';
            $('#<%=divItems.ClientID%>').append(str);
            
        }
        function test() {
            $.ajax({
                type: "POST",
                url: "PMeetingDetail.aspx/updateInfo",
                data: '{}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    if (result.hasOwnProperty("d")) { result = result.d; }
                    var tt = result.Desc;
                    $('#<%=tbIndo.ClientID%>').val(tt);
                    
                },
                error: function () { alert("Got Error!!"); }
            });
        }
    </script>  
 </asp:Content>
 <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="updatepanle1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
                 <div class="tab-content">
                    <div class="tab-pane active" id="Info">
                        <h2>
                            Meeting Information</h2>                
                        <hr />
                        <div class="offset1">
                            <table>
                                <tr>
                                    <td width="200px">
                                        Meeting Title
                                    </td>
                                    <td>
                                        <input type="text" runat="server" id="tbtitle2" value="" readonly/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Date
                                    </td>
                                    <td>
                                        <input type="text" runat="server" id="tbdate2" readonly/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Start Time
                                    </td>
                                    <td>
                                        <input id="tbSTime" type="text" runat="server" value="8:00" readonly/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        End Time
                                    </td>
                                    <td>
                                        <input type="text" id="tbETime" runat="server" value="10:00" readonly/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Description
                                    </td>
                                    <td>
                                        <textarea rows="5" cols="1" id="tbIndo2" readonly runat="server"/>
                                    </td>
                                </tr>
                        </div>
                        </table></div> 
                    <div align="center"><button id="btnEdit1" runnat="server" data-toggle="modal" data-target="#myModal1" onclick="getInfo()" class="btn btn-success">Edit</button></div>
                            <hr />
                            <h2>Participants</h2> <hr />
                    <div class="offset1">
                        <div class="container-fluid">
                            <div class="row-fluid">
                               
                                <div class="span4" style="border-right: 1px solid gray">
                                        Chairman                     
                                </div>
                                <div class="span8" >                                   
                                       <b>Attendees</b>                                        
                                </div>
                            </div>
                        </div>
                    </div>
                    <div align="center"><button id="btnEdit2" runnat="server" data-toggle="modal" data-target="#myModal2" onclick="getInfo()" class="btn btn-success">Edit</button></div>
                    <hr />
                    <div class="tab-pane" id="Resources">
                    <h2>
                            Location</h2>
                        <hr />
                        <div class="offset1">
                            <table>
                                <tr>
                                    <td width="130px">
                                        <b>Meeting Avenue:</b>
                                    </td>
                                    <td>
                                        <textbox></textbox>
                                    </td>
                                    <td>
                                        <div class="progress progress-indeterminate" id="progressDiv" hidden="hidden" style="display: none;">
                                            <div class="win-ring small">
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        </div>
                    <div align="center"><button id="btnEdit3" runnat="server" data-toggle="modal" onclick="getInfo()" data-target="#myModal3" class="btn btn-success">Edit</button></div>
                        <hr /> 
                    <h2>Resources</h2><hr />
                    <div class="offset1">
                                <h4>
                                    Current resources :</h4>
                            </div>
                    <div align="center"><button id="btnEdit4" runnat="server" data-toggle="modal" onclick="getInfo()" data-target="#myModal4" class="btn btn-success">Edit</button></div>
                      <br />
                      </div>
    <!-- Modal -->
           <div id="myModal1" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
            </button>
            <h3 id="myModalLabel">
                Meeting Information</h3>
        </div>
        <div class="modal-body">
            <div class="offset1">
                            <table>
                                <tr>
                                    <td width="200px">
                                        Meeting Title
                                    </td>
                                    <td>
                                        <input type="text" runat="server" id="tbtitle" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Date
                                    </td>
                                    <td>
                                        <input type="text" runat="server" id="tbdate" class="datepicker" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Start Time
                                    </td>
                                    <td>
                                        <input id="tbSTime2" type="text" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        End Time
                                    </td>
                                    <td>
                                        <input type="text" id="tbETime2" runat="server"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Description
                                    </td>
                                    <td>
                                        <textarea rows="5" cols="1" id="tbIndo" runat="server"></textarea>
                                    </td>
                                </tr>
                            </table>
        </div>
        </div>
        <div class="modal-footer">
            <button id="btnSave1" class="btn btn-primary" runat="server" onclick="test()"> Save</button>
            <button id="btnCancel" class="btn" data-dismiss="modal" aria-hidden="true">Cancel</button>
        </div>
    </div>
     <!--modal 2-->
           <div id="myModal2" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel2" aria-hidden="true">
        <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
            </button>
            <h3 id="myModalLabel2">
            Participants</h3></div>
            <div class="modal-body">
                        <div class="container-fluid">
                            <div class="row-fluid">
                               
                                <div class="span4" style="border-right: 1px solid gray">
                                        Chairman
                                   
                                </div>
                                <div class="span8" >
                                    
                                        <b>Attendees</b>
                                   </div> 
                                </div>
                            </div>
                            </div>
                 <div class="modal-footer">
            <button id="btnSave2" class="btn btn-primary" onclick="return redirect()"> Save</button>
            <button id="btnCancel2" class="btn" data-dismiss="modal" aria-hidden="true">Cancel</button>           
                  </div>
            </div>
     <!--modal3-->
           <div id="myModal3" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel3" aria-hidden="true">
        <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
            </button>
            <h3 id="myModalLabel3">
                 Location</h3></div>
                        <div class="modal-body">
                            <table>
                                <tr>
                                    <td width="130px">
                                        Meeting Avenue:
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlLocation" runat="server" onChange="checkAvl()" AppendDataBoundItems="True">
                                            <asp:ListItem Value="0">Choose a Room</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <div class="progress progress-indeterminate" id="Div2" hidden="hidden" style="display: none;">
                                            <div class="win-ring small"></div>
                                            
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                        <div>
                                            <span id="Span1"></span><a href="" id="a1" hidden="hidden" style="font-size: x-small">
                                                Show available rooms</a>
                                        </div>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </div>
                   <div class="modal-footer">
            <button id="btnSave3" class="btn btn-primary" onclick="return redirect()"> Save</button>
            <button id="btnCancel3" class="btn" data-dismiss="modal" aria-hidden="true">Cancel</button>      
                        
                  </div>
            </div>
     <!--modal4-->
           <div id="myModal4" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel4" aria-hidden="true">
       <div class="modal-header">
       <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
            </button>
            <h3 id="myModalLabel4">Resources</h3>
            </div>
            <div class="modal-body">
            <div class="offset1">
                            <div class="row-fluid">
                                <h4>
                                    Add resources</h4>
                                <br />
                                <table>
                                    <tr>
                                        <td width="130px">
                                            Item:
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlItems" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Quantity:
                                        </td>
                                        <td>
                                            <input type="text" id="tbQuantity" runat="server" value="1" />
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <input id="btnAdd" type="button" value="Add" class="btn btn-success" onclick="addItem()" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                    </div>
                    <div class="row-fluid">
                                <h4>
                                    Current resources</h4>
                            </div>
                            <div class="span6" id="divItems" runat="server">
                                
                               
                            </div>
                            </div>
                    <div class="modal-footer">
                    <button id="btnSave4" class="btn btn-primary" onclick="return redirect()">Save</button>
                    <button id="btnCancel4" class="btn" data-dismiss="modal" aria-hidden="true">Cancel</button>
                    </div>
                        </div>
  
                    
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
