namespace Chords_site.Models
{
    public class Chord
    {
        public Chord(int id, string name, string diagram_url)
        {
            Id = id;
            Name = name;
            Diagram_url = diagram_url;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Diagram_url  { get; set; }
    }
}
