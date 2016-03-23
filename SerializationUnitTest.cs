﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gramma.Serialization.Testing.MusicModel;
using System.IO;
using System.Runtime.Serialization;

namespace Gramma.Serialization.Testing
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class SerializationUnitTest
	{
		public SerializationUnitTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void SerializaAndDeserializeAllObjectTypes()
		{
			var serializedObject = BuildAlbumArray();

			var formatter = new FastBinaryFormatter();

			var surrogateSelector = new SurrogateSelector();

			surrogateSelector.AddSurrogate(typeof(Genre), new StreamingContext(), new GenreSerializationSurrogate());

			formatter.SurrogateSelector.ChainSelector(surrogateSelector);

			using (var stream = new MemoryStream())
			{
				formatter.Serialize(stream, serializedObject);

				stream.Seek(0, SeekOrigin.Begin);

				var deserializedObject = (Album[])formatter.Deserialize(stream);

				foreach (var album in deserializedObject)
				{
					var albumGenre = album.Genre;

					if (albumGenre != null)
					{
						Assert.AreSame(albumGenre, Genre.Get(albumGenre.Key));
					}

					Assert.AreEqual(album.Edition.MajorNumber, 1);
				}
			}
			
		}

		[TestMethod]
		[ExpectedException(typeof(SerializationException))]
		public void DiscoverMissingSerializableAttribute()
		{
			var serializedObject = this;

			var formatter = new FastBinaryFormatter();

			using (var stream = new MemoryStream())
			{
				formatter.Serialize(stream, serializedObject);
			}
		}

		private Album[] BuildAlbumArray()
		{
			var albums = new List<Album>();

			var grammophoneArtist = new Artist("grammophone", Genre.Get("electronic"));

			var lostDogArtist = new Artist("Lost Dog", Genre.Get("pop"));

			var grammophoneSongs = new List<Song>();

			grammophoneSongs.Add(new Song("clear day", grammophoneArtist, Genre.Get("electronic")));
			grammophoneSongs.Add(new Song("pure talentless", grammophoneArtist, Genre.Get("electronic")));

			var edition = new Edition { MajorNumber = 1, MinorNumber = 0 };

			var grammophoneAlbum = new Album(grammophoneArtist, Genre.Get("electronic"), grammophoneSongs, Packaging.Download, edition);

			var lostDogSongs = new List<Song>();

			lostDogSongs.Add(new Song("Idleness", lostDogArtist, Genre.Get("electronic")));
			lostDogSongs.Add(new Song("Catch a bone", lostDogArtist, Genre.Get("pop")));

			var lostDogAlbum = new Album(lostDogArtist, Genre.Get("pop"), lostDogSongs, Packaging.CDOrDownload, edition);

			return new Album[] { grammophoneAlbum, lostDogAlbum };
		}

	}
}
