<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TheBox.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<style type="text/css">
html,body
{
    background-image:url('bkg-4.png');
    background-size:1300px 700px;
    overflow-y:hidden;
    overflow-x:auto;
    margin:0px;width:100%; height:100%;
}
</style>
    <link href="content/css/MetroJs.css" rel="stylesheet" type="text/css" />
    <link href="content/css/MetroJs.min.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
    
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="content/js/jquery-1.9.1.js" type="text/javascript"></script>
    <script src="content/js/jquery-ui.js" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function () {
        $(".live-tile").liveTile();
        // jQuery UI 
        // http://jqueryui.com/sortable/#display-grid
        $(".panorama-section").sortable();
        $(".panorama-section").disableSelection();
        });
    </script>
    <div class="container-fluid">
      <div class="row">
      
         <div id="hub" class="metro span12 offset2">
            <div class="panorama" style="height: 770px;">
               <div style="width: 1430px; margin-left: -715px; height: 770px;" class="panorama-sections">
   
               <div id="section1" class="panorama-section tile-span-4">
   
                  <h2>UTC Dashboard</h2>
   
                  <a class="tile wide imagetext bg-color-orange" href="./metro-layouts.html">
                     <div class="image-wrapper">
                        <img src="../content/img/metro-tiles.jpg"/>
                     </div>
                     <div class="column-text">
                        <div class="text4">Demos of various Metro style layouts.</div>
                     </div>
                     <div class="app-label">Layouts</div>
                  </a>
   
                  <a class="tile wide imagetext bg-color-blueDark" href="./tiles-templates.html">
                     <div class="image-wrapper">
                        <img src="../content/img/metro-tiles.jpg"/>
                     </div>
                     <div class="column-text">
                        <div class="text4">List of all tile templates: square, wide, widepeek, with images or text only.</div>
                     </div>
                     <div class="app-label">Tiles Templates</div>
                  </a>
   
                  <a class="tile wide imagetext wideimage bg-color-orange" href="./appbar-demo.html">
                     <img src="../content/img/appbar.png" alt=""/>
                     <div class="textover-wrapper bg-color-blue">
                        <div class="text2">Application Bar</div>
                     </div>
                  </a>
   
                  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a class="tile app square bg-color-greenDark" href="./table.html"><div class="image-wrapper">
                        <img src="content/img/My Apps.png" alt="" />
                     </div>
                     <div class="app-label">Styled Table</div>
                  </a>
   
                  <a class="tile app bg-color-purple" href="./listviews.html">
                     <div class="image-wrapper">
                        <span class="icon icon-list-2"></span>
                     </div>
                     <span class="app-label">ListView</span>
                  </a>
   
                  <a class="tile app bg-color-red" href="#">
                     <div class="image-wrapper">
                        <span class="icon icon-publish"></span>
                     </div>
                     <span class="app-label">[TODO] Charms</span>
                  </a>
   
                  <a class="tile app bg-color-blueDark" href="./icons.html">
                     <div class="image-wrapper">
                        <span class="icon icon-IcoMoon"></span>
                     </div>
                     <span class="app-label">Icons</span>
                  </a>
   
                  <a class="tile app bg-color-purple" href="./metro-components.html">
                     <div class="image-wrapper">
                        <span class="icon icon-list-2"></span>
                     </div>
                     <span class="app-label">Metro Components</span>
                  </a>
   
               </div>
                
               
                  <br />
               <div id="section2" class="panorama-section tile-span-6 ">
               <br /><br /><br />
                   <%--<a class="tile wide imagetext bg-color-blueDark" href="#">
                     <div class="image-wrapper">
                        <img alt="" src="../content/img/Coding app.png"/>
                     </div>
                     <div class="column-text">
                        <div class="text">User</div>
                        <div class="text">Directory</div>
                     </div>
                     <span class="app-label">Directory</span>
                  </a>--%>
                  <a class="tile wide imagetext bg-color-blueDark" href="PMeetings.aspx">
                     <div class="image-wrapper">
                        <img src="../content/img/My Apps.png"/>
                     </div>
                    <div class="column-text">
                        <div class="text">Meeting</div>
                        <div class="text">Management</div>
                     </div>
                     <span class="app-label" id="meetingDiv" runat="server"></span>
                  </a>

                <a class="app tile full bg-color-orange" href="PProject.aspx">
                <div id="" class="live-tile myClass" data-mode="flip" data-stops="100%" data-delay="3500">
                    <span><b>Projects</b></span>
                    <div class="image-wrapper"><img src="../content/img/RegEdit.png" />
                    </div>
                    <div class="accent cobalt bg-color-orange"><br /><p><i>Create and Manage projects</i></p>
                    </div>
                </div>
                </a>
                  <a class="app tile bg-color-greenDark" href="#">
                 <div id="" class="live-tile" data-mode="flip" data-direction="horizontal" data-delay="2500" data-initdelay="2000">        
                    <span><b>Documents</b></span>
                    <div class="image-wrapper"><span class="icon icon-folder-open"></span><%--<img src="content/img/archive2.png" />--%>
                    </div>
                    <div class="accent cobalt bg-color-greenDark"><br /><p><i>Upload, View and Edit Files</i>
                    </p></div>
                </div></a>               
                     
                  <a class="app tile bg-color-blue" href="PCalender.aspx">
                 <div id="" class="live-tile" data-mode="flip" data-direction="horizontal" data-delay="4500" data-initdelay="1000">        
                    <span class=""><b>Calendar</b></span>
                    <div class="image-wrapper">
                    <span class="icon icon-calendar-4"></span>
                    </div>
                    <div class="accent cobalt bg-color-blue"><br /><p><i>Manage your schedule, appointments and events through the Calendar</i>
                    </p></div>
                </div></a>  
<%--                   </div>
                        <h2></h2>
                   <div class="panorama-section tile-span-4">--%>                        
                        <a class="app tile bg-color-red" href="">
                        <div id="" class="live-tile" data-mode="flip"data-delay="4500" data-initdelay="2000">        
                        <span class=""><b>Contacts</b></span>
                        <div class="image-wrapper"><span class="icon icon-users"></span>
                        </div>
                        <div class="accent cobalt bg-color-red"><br /><p><i>Manage your contacts</i></p></div>
                        </div></a> 
   
                        <a href="Update.aspx" class="tile wide imagetext bg-color-purple">
                           <div class="image-wrapper">
                              <span class="icon icon-chat-2"></span>
                           </div>
                           <div class="column-text">
                              <div class="text4">Notifications</div>
                           </div>
                           <div class="app-label" runat="server" id="NotDiv">You have 20 new notifications</div>
                        </a>

                       <%--<a href="PCalender.aspx" class=" tile wide tile app bg-color-green">
                           <div class="image-wrapper">
                              <span class="icon icon-calendar-2"></span>
                           </div>
                           <div class="app-label">Calender</div>
                        </a>--%>

                         <%--<a class="tile app bg-color-blueDark" href="#">
                           <div class="image-wrapper">
                              <span class="icon icon-statistics"></span>
                           </div>
                           <div class="app-label">Statistics</div>
                        </a> --%>

                     </div>
                     </div>
            </div>
      
         </div>
      </div></div>
    <script src="content/js/google-code-prettify/prettify.css" type="text/javascript"></script>
    <script src="content/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="content/js/bootmetro-panorama.js" type="text/javascript"></script>
    <script src="content/js/bootmetro-pivot.js" type="text/javascript"></script>
    <script src="content/js/bootmetro-charms.js" type="text/javascript"></script>
    <script src="content/js/bootstrap-datepicker.js" type="text/javascript"></script>
    <script src="content/js/jquery.touchSwipe.js" type="text/javascript"></script>
    <script src="content/js/demo.js" type="text/javascript"></script>
    <script src="content/js/holder.js" type="text/javascript"></script>
    <script src="content/js/MetroJs.js" type="text/javascript"></script>
    <script src="content/js/MetroJs.min.js" type="text/javascript"></script>
    </asp:Content>

