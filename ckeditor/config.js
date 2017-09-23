/*
Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function( config ) {
    config.extraPlugins = 'savebtn'; //savebtn is the plugin's name
    config.saveSubmitURL = 'http://localhost:7876/WebForm1.aspx/test/'; //link to serverside script to handle the post
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';
};
