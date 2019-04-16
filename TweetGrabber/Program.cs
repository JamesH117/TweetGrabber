using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace TweetGrabber
{
    class Program
    {
        static void Main(string[] args)
        {

            DataFilter tweet_grabber = new DataFilter();

            DateTime start_date = new DateTime(2016, 1, 1);

            DateTime end_date = new DateTime(2017, 12, 31);

            List<TweetDict> response = tweet_grabber.getTweets(start_date, end_date);
        }

    }
}
