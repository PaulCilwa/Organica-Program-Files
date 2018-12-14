// Requires <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
"use strict";

var MyOrganicaAudio = new OrganicaAudio();

function OrganicaAudio()
	{
	console.log ('OrganicaAudio initializing...');
	
	var contextClass = (window.AudioContext || 
		window.webkitAudioContext || 
		window.mozAudioContext || 
		window.oAudioContext || 
		window.msAudioContext);
	  
	if (contextClass) 
		{
		// Web Audio API is available.
		this.Context = new contextClass();
		this.Playlist = [];
		console.log('OrganicaAudio: Hello');
		}
	else
		{
		// Trigger error??
		console.log('OrganicaAudio: Unable to obtain Web Audio API context.');
		}
	}
	
OrganicaAudio.prototype.AddTrack = function(aSource, anID)
	{
	switch(aSource)
		{
		case 'audio':
			if (typeof(anID) == "undefined")
				aSource = $("audio > source").first().attr("src")
			else
				aSource = $(anID + '> source').first().attr("src");
			break;
		}
	this.PlaylistIndex = this.Playlist.push(new OrganicaAudioTrack(aSource)) - 1;
	if (this.PlaylistIndex > 0)
		this.Playlist[this.PlaylistIndex-1].ToPlayNext = this.Playlist[this.PlaylistIndex]
	}
	
OrganicaAudio.prototype.Play = function()
	{
	console.log(this.Playlist[0]);
	this.Playlist[0].Play();
	}
	

	
	