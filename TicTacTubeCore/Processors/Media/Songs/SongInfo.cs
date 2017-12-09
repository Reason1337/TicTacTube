﻿using TagLib;

namespace TicTacTubeCore.Processors.Media.Songs
{
	/// <summary>
	///     Info for a given song.
	/// </summary>
	public struct SongInfo : IMediaInfo
	{
		/// <summary>
		///     The title of the song.
		/// </summary>
		public string Title;

		/// <summary>
		///     The contributing artists.
		/// </summary>
		public string[] Artists;

		/// <summary>
		///     The album of the song.
		/// </summary>
		public string Album;

		/// <summary>
		///     The genres of the song.
		/// </summary>
		public string[] Genres;

		/// <summary>
		///     The year this song was released.
		/// </summary>
		public uint Year;

		/// <summary>
		///     The bitrate in kbps.
		/// </summary>
		public int Bitrate;

		/// <summary>
		/// The pictures that belong to the song (e.g. cover art).
		/// </summary>
		public IPicture[] Pictures;

		//TODO: cover art
		//TODO:lyrics?
		/// <inheritdoc />
		public void WriteToFile(string path)
		{
			using (var f = File.Create(path))
			{
				f.Tag.Title = Title;
				f.Tag.Performers = Artists;
				f.Tag.Album = Album;
				f.Tag.Genres = Genres;
				f.Tag.Year = Year;
				f.Tag.Pictures = Pictures;

				f.Tag.Comment = "";

				f.Save();
			}
		}

		/// <summary>
		/// Read the songinfo from a given file.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static SongInfo ReadFromFile(string path)
		{
			var songInfo = new SongInfo();

			using (var f = File.Create(path))
			{
				songInfo.Title = f.Tag.Title;
				songInfo.Artists = f.Tag.Performers;
				songInfo.Album = f.Tag.Album;
				songInfo.Genres = f.Tag.Genres;
				songInfo.Year = f.Tag.Year;
				songInfo.Bitrate = f.Properties.AudioBitrate;
				songInfo.Pictures = f.Tag.Pictures;
			}

			return songInfo;
		}
	}
}