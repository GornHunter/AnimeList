using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AnimeList.Model
{
    internal class Anime
    {
        public string Id { get; set; }
        public Uri Picture { get; set; }
        public string Title { get; set; }

        public Anime(Uri Picture, string Title) 
        {
            this.Picture = Picture;
            this.Title = Title;
        }

        public Anime(string Id, Uri Picture, string Title)
        {
            this.Id = Id;
            this.Picture = Picture;
            this.Title = Title;
        }
    }
}
