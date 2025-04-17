using NLog;
using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        string path = Directory.GetCurrentDirectory() + "//nlog.config";
        var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

        logger.Info("Program started");

        var db = new DataContext();
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nChoose an option:");
            Console.WriteLine("1) Display all blogs");
            Console.WriteLine("2) Add Blog");
            Console.WriteLine("3) Create Post");
            Console.WriteLine("4) Display Posts");
            Console.WriteLine("5) Exit");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    var blogsToDisplay = db.Blogs.OrderBy(b => b.Name);
                    Console.WriteLine("All blogs in the database:");
                    foreach (var item in blogsToDisplay)
                    {
                        Console.WriteLine(item.Name);
                    }
                    break;

                case "2":
                    Console.Write("Enter a name for a new Blog: ");
                    var name = Console.ReadLine();
                    var blog = new Blog { Name = name };
                    db.AddBlog(blog);
                    logger.Info("Blog added - {name}", name);
                    break;

                case "3":
                    try
                    {
                        var blogs = db.Blogs.OrderBy(b => b.Name).ToList();
                        if (!blogs.Any())
                        {
                            Console.WriteLine("No blogs available.");
                            break;
                        }

                        Console.WriteLine("Select a blog to post to:");
                        for (int i = 0; i < blogs.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {blogs[i].Name}");
                        }

                        if (!int.TryParse(Console.ReadLine(), out int blogChoice) || blogChoice < 1 || blogChoice > blogs.Count)
                        {
                            Console.WriteLine("Invalid selection.");
                            break;
                        }

                        var selectedBlog = blogs[blogChoice - 1];

                        Console.Write("Enter post title: ");
                        var title = Console.ReadLine();

                        Console.Write("Enter post content: ");
                        var content = Console.ReadLine();

                        var post = new Post { Title = title, Content = content, BlogId = selectedBlog.BlogId };
                        db.Posts.Add(post);
                        db.SaveChanges();

                        logger.Info("Post added - {title}", title);
                        Console.WriteLine("Post created successfully!");
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Error creating post");
                        Console.WriteLine("An error occurred while creating the post.");
                    }
                    break;

                case "4":
                    var allBlogs = db.Blogs.OrderBy(b => b.Name).ToList();
                    if (!allBlogs.Any())
                    {
                        Console.WriteLine("No blogs available.");
                        break;
                    }

                    Console.WriteLine("Select a blog to view posts:");
                    for (int i = 0; i < allBlogs.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}) {allBlogs[i].Name}");
                    }

                    if (!int.TryParse(Console.ReadLine(), out int selectedIndex) || selectedIndex < 1 || selectedIndex > allBlogs.Count)
                    {
                        Console.WriteLine("Invalid selection.");
                        break;
                    }

                    var selected = allBlogs[selectedIndex - 1];
                    var posts = db.Posts.Where(p => p.BlogId == selected.BlogId).ToList();

                    Console.WriteLine($"\n{posts.Count} posts found for blog '{selected.Name}':\n");
                    foreach (var post in posts)
                    {
                        Console.WriteLine($"Blog: {selected.Name}");
                        Console.WriteLine($"Title: {post.Title}");
                        Console.WriteLine($"Content: {post.Content}\n");
                    }
                    break;

                case "5":
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Invalid option. Please choose 1-5.");
                    break;
            }
        }

        logger.Info("Program ended");
    }
}