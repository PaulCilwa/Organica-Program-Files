// Requires <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
"use strict";

/*****************************************************************************
/*
/*	OrganicaAudio
/*
/*****************************************************************************/

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
	
/*****************************************************************************
/*
/*	OrganicaAudioTrack
/*
/*****************************************************************************/

function OrganicaAudioTrack(aSource)
	{
	this.Context = MyOrganicaAudio.Context;
	this.Filename = aSource;
	console.log(this.Filename);
	this.StartCrossFade = 4;
	this.Loaded = false;
    this.Loading = false;
    this.Playing = false;
	}
	
OrganicaAudioTrack.prototype.Load = function()
	{
console.log("Load");
	var Me = this;
	Me.Loading = true;
	return new Promise(
		function(resolve, reject) 
			{
			console.log ("Creating a Promise...");
		    var Request = new XMLHttpRequest();
		    Request.open("GET", Me.Filename, true);
		    Request.responseType = "arraybuffer";
			Request.onerror = () => { alert('BufferLoader: XHR error'); }
			Request.onload = function()
		    	{
				if (Request.response) 
					{
					console.log("Request completed");
					Me.Context.decodeAudioData(Request.response, function (Result)
						{
						console.log("Decoded...");
						Me.SoundSource = MyOrganicaAudio.Context.createBufferSource();
						Me.SoundSource.buffer = Result;
						Me.Duration = Result.duration;
						Me.Loading = false;
					    Me.Loaded = true;
						resolve(Me);
						},
					function ()
						{
						reject(Me.Filename);
						});
					}
				else 
					reject("Disaster! " + Me.Filename); 
				}
		    Request.send();
		    console.log("Request sent...");
			}
	    );
	};
	
OrganicaAudioTrack.prototype.Play = function(StartTime)
	{
	var Me = this;
	
console.log("Play!");
	if (isNaN(StartTime)) 
		StartTime = 0; 
		
console.log("StartTime = " + StartTime);
	if (! this.Loaded)
		{
		this.Load().then(function(Me) { Me.Play( StartTime); } );
		return;
		}
	else
		{
		//Actually play the damned thing...
		this.SoundSource.onended = function()
			{
			console.log("Ended!");
			Me.Playing = false;
			};
		this.SoundSource.connect(this.Context.destination);
		this.Playing = true;
		this.SoundSource.start(StartTime + Me.Context.currentTime);
		this.PlayNext();
		}
	}

OrganicaAudioTrack.prototype.PlayNext = function()
	{
	console.log("Play next...");
	console.log(this);
	
	if (this.ToPlayNext === undefined)
		{
		console.log("Playlist empty.");
		return;
		}
	else 
		console.log("Something to play...");
	
	this.ToPlayNext.Play(this.Duration - this.StartCrossFade);
	}
	

	
	