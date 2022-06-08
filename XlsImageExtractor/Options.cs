namespace XlsImageExtractor
{
    public class Options
    {
        public enum Renderer
        {
            defaultRenderer,
            xPRenderer,
            tileRenderer,
            zoomingRenderer,
            noirRenderer
        }

        static public int thumbnailSize { get; set; } = 96;
        static public Renderer renderer { get; set; } = Renderer.defaultRenderer;
        static public string language { get; set; } = "ko-KR";
    }
}
