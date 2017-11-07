using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grammophone.Serialization.Testing.MusicModel
{
	[Serializable]
	public struct Edition
	{
		public int MajorNumber;

		public int MinorNumber;

        public DateTime ReleaseDate { get; set; }
    }
}
