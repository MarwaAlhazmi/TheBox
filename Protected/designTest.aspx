<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="designTest.aspx.cs" Inherits="TheBox.Protected.designTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
   <meta charset="utf-8">
   <!-- Always force latest IE rendering engine (even in intranet) & Chrome Frame -->
   <meta content="IE=edge,chrome=1" http-equiv="X-UA-Compatible">
   <!-- Mobile viewport optimized: h5bp.com/viewport -->
   <meta content="width=device-width" name="viewport">

   <title>Hub · BootMetro</title>

   <meta content="noindex, nofollow" name="robots">
   <meta content="BootMetro : Simple and complete web UI framework to create web apps with Windows 8 Metro user interface." name="description">
   <meta content="bootmetro, modern ui, modern-ui, metro, metroui, metro-ui, metro ui, windows 8, metro style, bootstrap, framework, web framework, css, html" name="keywords">
   <meta content="AozoraLabs by Marcello Palmitessa" name="author">
   
   <!-- remove or comment this line if you want to use the local fonts -->
   <link type="text/css" rel="stylesheet" href="http://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700">

   <!--<link rel="stylesheet" type="text/css" href="content/css/bootstrap.css">-->
   <link href="../content/css/bootmetro-icons.css" type="text/css" rel="stylesheet">
   <link href="../content/css/bootmetro.css" type="text/css" rel="stylesheet">
   <link href="../content/css/bootmetro-responsive.css" type="text/css" rel="stylesheet">
   <link href="../content/css/bootmetro-ui-light.css" type="text/css" rel="stylesheet">
   <link href="../content/css/datepicker.css" type="text/css" rel="stylesheet">
   <!--[if IE 7]>
   <link rel="stylesheet" type="text/css" href="content/css/bootmetro-icons-ie7.css">
   <![endif]-->

   <!--  these two css are to use only for documentation -->
   <link href="../content/css/demo.css" type="text/css" rel="stylesheet">
   <link href="../Scripts/google-code-prettify/prettify.css" type="text/css" rel="stylesheet">

   <!-- Le fav and touch icons -->
   <link href="../content/ico/favicon.ico" rel="shortcut icon">
   <link href="../content/ico/apple-touch-icon-144-precomposed.png" sizes="144x144" rel="apple-touch-icon-precomposed">
   <link href="../content/ico/apple-touch-icon-114-precomposed.png" sizes="114x114" rel="apple-touch-icon-precomposed">
   <link href="../content/ico/apple-touch-icon-72-precomposed.png" sizes="72x72" rel="apple-touch-icon-precomposed">
   <link href="../content/ico/apple-touch-icon-57-precomposed.png" rel="apple-touch-icon-precomposed">
  
   <!-- All JavaScript at the bottom, except for Modernizr and Respond.
      Modernizr enables HTML5 elements & feature detects; Respond is a polyfill for min/max-width CSS3 Media Queries
      For optimal performance, use a custom Modernizr build: www.modernizr.com/download/ -->
   <script type="text/javascript" async="" src="http://www.google-analytics.com/ga.js"></script><script src="../Scripts/modernizr-2.6.2.min.js"></script>

   <script type="text/javascript">
       var _gaq = _gaq || [];
       _gaq.push(['_setAccount', 'UA-3182578-6']);
       _gaq.push(['_trackPageview']);
       (function () {
           var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
           ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
           var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
       })();
   </script>
</head>


<body>
   <!--[if lt IE 7]>
   <p class="chromeframe">You are using an <strong>outdated</strong> browser. Please <a href="http://browsehappy.com/">upgrade your browser</a> or <a href="http://www.google.com/chromeframe/?redirect=true">activate Google Chrome Frame</a> to improve your experience.</p>
   <![endif]-->

   <!-- Header
   ================================================== -->
   <header class="container" id="nav-bar">
      <div class="row">
         <div class="span12">
            <div id="header-container">
               <a href="./index.html" class="win-backbutton" id="backbutton"></a>
               <h5>UTC Metro Screen</h5>
               <div class="dropdown">
                  <a href="#" data-toggle="dropdown" class="header-dropdown dropdown-toggle accent-color">
                     Start
                     <b class="caret"></b>
                  </a>
                  <ul class="dropdown-menu">
                  <li><a href="./metro-layouts.html">Metro Layouts</a></li>
                  <li><a href="./hub.html">Hub</a></li>
                  <li><a href="./tiles-templates.html">Tile Templates</a></li>
                  <li><a href="./listviews.html">ListViews</a></li>
                  <li><a href="./appbar-demo.html">App-Bar Demo</a></li>
                  <li><a href="./table.html">Table Demo</a></li>
                  <li><a href="./wizard.html">Wizard</a></li>
                  <li><a href="./icons.html">Icons</a></li>
                  <li><a href="./toast.html">Toast Notifications</a></li>
                  <li><a href="./pivot.html">Pivot</a></li>
                  <li><a href="./metro-components.html">Metro Components</a></li>
                  <li class="divider"></li>
                  <li><a href="./scaffolding.html">Bootstrap Scaffolding</a></li>
                  <li><a href="./base-css.html">Bootstrap Base CSS</a></li>
                  <li><a href="./components.html">Bootstrap Components</a></li>
                  <li><a href="./javascript.html">Bootstrap Javascript</a></li>
                  <li class="divider"></li>
                  <li><a href="./index.html">Home</a></li>
               </ul>
            </div>
            </div>
            <div class="pull-right" id="top-info">
             <a id="settings" href="#" class="win-command pull-right">
               <span class="win-commandicon win-commandring icon-cog-3"></span>
            </a>

            
            <a id="logged-user" href="#" class="win-command pull-right">
               <span class="win-commandicon win-commandring icon-user"></span>
            </a>
            <!--the 
            <a href="#" class="pull-right" id="settings">
               <b class="icon-settings"></b>
            </a>new -->

   
            
            <!--<hr class="separator 
            <a class="pull-right" href="#" id="logged-user">
                  <b class="icon-user-2"></b>pull-right"/>-->
            </a>
            <div class="pull-left">
               <h3>FirstName</h3>
               <h4>LastName</h4>
            </div>
         </div>
      </div>
      </div>
   </header>
   
   <div class="container">
      <div class="row">
         <div class="metro span12" id="hub">
            <div class="panorama">
               <div class="panorama-sections" style="width: 1430px; margin-left: -715px;">
   
               <div class="panorama-section tile-span-4" id="section1">
   
                  <h2>UTC Dashboard</h2>
   
                  <a href="./metro-layouts.html" class="tile wide imagetext bg-color-orange">
                     <div class="image-wrapper">
                        <img src="../content/img/metro-tiles.jpg">
                     </div>
                     <div class="column-text">
                        <div class="text4">Demos of various Metro style layouts.</div>
                     </div>
                     <div class="app-label">Layouts</div>
                  </a>
   
                  <a href="./tiles-templates.html" class="tile wide imagetext bg-color-blueDark">
                     <div class="image-wrapper">
                        <img src="../content/img/metro-tiles.jpg">
                     </div>
                     <div class="column-text">
                        <div class="text4">List of all tile templates: square, wide, widepeek, with images or text only.</div>
                     </div>
                     <div class="app-label">Tiles Templates</div>
                  </a>
   
                  <a href="./appbar-demo.html" class="tile wide imagetext wideimage bg-color-orange">
                     <img alt="" src="../content/img/appbar.png">
                     <div class="textover-wrapper bg-color-blue">
                        <div class="text2">Application Bar</div>
                     </div>
                  </a>
   
                  <a href="./table.html" class="tile app square bg-color-greenDark">
                     <div class="image-wrapper">
                        <img alt="" src="content/img/My Apps.png">
                     </div>
                     <div class="app-label">Styled Table</div>
                  </a>
   
                  <a href="./listviews.html" class="tile app bg-color-purple">
                     <div class="image-wrapper">
                        <span class="icon icon-list-2"></span>
                     </div>
                     <span class="app-label">ListView</span>
                  </a>
   
                  <a href="#" class="tile app bg-color-red">
                     <div class="image-wrapper">
                        <span class="icon icon-publish"></span>
                     </div>
                     <span class="app-label">[TODO] Charms</span>
                  </a>
   
                  <a href="./icons.html" class="tile app bg-color-blueDark">
                     <div class="image-wrapper">
                        <span class="icon icon-IcoMoon"></span>
                     </div>
                     <span class="app-label">Icons</span>
                  </a>
   
                  <a href="./metro-components.html" class="tile app bg-color-purple">
                     <div class="image-wrapper">
                        <span class="icon icon-list-2"></span>
                     </div>
                     <span class="app-label">Metro Components</span>
                  </a>
   
               </div>
   
               <div class="panorama-section tile-span-4" id="section2">
   
                  <h2>Bootstrap Metro</h2>
   
                  <a href="./scaffolding.html" class="tile wide imagetext bg-color-blue">
                     <div class="image-wrapper">
                        <img src="../content/img/My Apps.png">
                     </div>
                     <div class="column-text">
                        <div class="text">Grid system</div>
                        <div class="text">Layouts</div>
                        <div class="text">Responsive design</div>
                     </div>
                     <span class="app-label">SCAFFOLDING</span>
                  </a>
   
                  <a href="./base-css.html" class="tile wide imagetext bg-color-blueDark">
                     <div class="image-wrapper">
                        <img src="../content/img/Coding app.png" alt="">
                     </div>
                     <div class="column-text">
                        <div class="text">Typography</div>
                        <div class="text">Tables</div>
                        <div class="text">Forms</div>
                        <div class="text">Buttons</div>
                     </div>
                     <span class="app-label">BASE CSS</span>
                  </a>
   
                  <a href="./components.html" class="tile app bg-color-orange">
                     <div class="image-wrapper">
                        <img alt="" src="../content/img/RegEdit.png">
                     </div>
                     <span class="app-label">COMPONENTS</span>
                  </a>
   
                  <a href="./javascript.html" class="tile app bg-color-red">
                     <div class="image-wrapper">
                        <img alt="" src="../content/img/Devices.png">
                     </div>
                     <span class="app-label">JAVASCRIPT</span>
                  </a>
   
               </div>
   
            </div>
            </div>
            <a href="#" id="panorama-scroll-prev" style="display: none;">&#xe05d;</a>
            
            <a href="#" id="panorama-scroll-next" style="display: block;">&#xe05d;</a>
         </div>
      </div>
   </div>
   
   
   <div class="win-ui-dark slide" id="charms">
   
      <div class="charms-section" id="theme-charms-section">
         <div class="charms-header">
            <a class="close-charms win-backbutton" href="#"></a>
            <h2>Settings</h2>
         </div>
   
         <div class="row-fluid">
            <div class="span12">
   
               <form class="">
                  <label for="win-theme-select">Change theme:</label>
                  <select class="" id="win-theme-select">
                     <option value="metro-ui-light">Light</option>
                     <option value="metro-ui-dark">Dark</option>
                  </select>
               </form>
   
            </div>
         </div>
      </div>
   
   </div>

   <!-- Grab Google CDN's jQuery. fall back to local if necessary -->
   <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
   <script type="text/javascript">       window.jQuery || document.write("&lt;script src='scripts/jquery-1.8.3.min.js'&gt;\x3C/script&gt;")</script>

   <script src="../Scripts/google-code-prettify/prettify.js" type="text/javascript"></script>
   <script src="../Scripts/bootstrap.min.js" type="text/javascript"></script>
   <script src="../Scripts/bootmetro-panorama.js" type="text/javascript"></script>
   <script src="../Scripts/bootmetro-pivot.js" type="text/javascript"></script>
   <script src="../Scripts/bootmetro-charms.js" type="text/javascript"></script>
   <script src="../Scripts/bootstrap-datepicker.js" type="text/javascript"></script>
   <!--<script type="text/javascript" src="scripts/jquery.nicescroll.js"></script>-->
   <script src="../Scripts/jquery.touchSwipe.js" type="text/javascript"></script>
   <script src="../Scripts/demo.js" type="text/javascript"></script>
   <script src="../Scripts/holder.js" type="text/javascript"></script>
   <script type="text/javascript">
       $('.panorama').panorama({
           nicescroll: false,
           showscrollbuttons: true,
           keyboard: true
       });

       $('#pivot').pivot();
   </script>


</body>
</html>
