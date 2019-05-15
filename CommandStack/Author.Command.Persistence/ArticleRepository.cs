using Author.Command.Domain.Command;
using System;
using System.Threading.Tasks;

namespace Author.Command.Persistence
{
    public class ArticleRepository
    {

        public async Task<CreateArticleCommandResponse> CreateArticleDetails()
        {
            //string query = "INSERT INTO ArticlesTBL (ArticleTitle, ArticleContent, ArticleType, ArticleImg, ArticleBrief,  ArticleDateTime, ArticleAuthor, ArticlePublished, ArticleHomeDisplay, ArticleViews)";
            //query += " VALUES (@ArticleTitle, @ArticleContent, @ArticleType, @ArticleImg, @ArticleBrief, @ArticleDateTime, @ArticleAuthor, @ArticlePublished, @ArticleHomeDisplay, @ArticleViews)";

            //string cmdString = "INSERT INTO books (name,author,price) VALUES (@val1, @va2, @val3)";
            //string connString = "your connection string";
            //using (SqlConnection conn = new SqlConnection(connString))
            //{
            //    using (SqlCommand comm = new SqlCommand())
            //    {
            //        comm.Connection = conn;
            //        comm.CommandString = cmdString;
            //        comm.Parameters.AddWithValue("@val1", txtbox1.Text);
            //        comm.Parameters.AddWithValue("@val2", txtbox2.Text);
            //        comm.Parameters.AddWithValue("@val3", txtbox3.Text);
            //        try
            //        {
            //            conn.Open();
            //            comm.ExecuteNonQuery();
            //        }
            //         catch(SqlException e)
            //        {
            //            // do something with the exception
            //            // don't hide it
            //        }
            //    }
            //}
            return new CreateArticleCommandResponse { };
        }
    }
}