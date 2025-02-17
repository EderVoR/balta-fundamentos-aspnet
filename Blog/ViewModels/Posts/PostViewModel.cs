namespace Blog.ViewModels.Posts
{
	public class PostViewModel
	{
        public int Id { get; set; }
		public string Title { get; set; }
		public string Slug { get; set; }
		public DateTime CreateDate { get; set; }
		public string Category { get; set; }
		public string Author { get; set; }
	}
}
