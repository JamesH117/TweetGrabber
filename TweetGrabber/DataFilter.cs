using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.IO;

namespace TweetGrabber
{
    class DataFilter
    {

        string base_url = "https://badapi.iqvia.io/api/v1/Tweets?startDate={0}&endDate={1}";

        public object JsonConvert { get; private set; }

        public List<TweetDict> getTweets(DateTime start_date, DateTime end_date)
        {

            List<TweetDict> response = RecTweetGrabber(start_date, end_date);


            //Console.WriteLine(response.Count);

            //Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(Directory.GetCurrentDirectory(), "\\" + "temp.csv")));
            //File.AppendAllText("temp.csv", response.ToString);

            
            string temp_name = String.Format("Solution for start_date[{0}] end_date[{1}].json", start_date.ToString("yyyy-MM-dd"), end_date.ToString("yyyy-MM-dd"));
            //string temp_name = "Solution.csv";
            Debug.WriteLine(String.Format("Creating file with name:[{0}]", temp_name));
            foreach (TweetDict some_dict in response)
            {
                DictToCsv(some_dict, temp_name);
            }

            // Output to a CSV file possibly?
            return response;
        }

        // Function to simply print out all Keys and Value pairs into Debug Console
        private void DebugListDict(List<Dictionary<string, string>> list_dict)
        {

            foreach (Dictionary<string, string> some_dict in list_dict)
            {
                foreach (KeyValuePair<string, string> kvp in some_dict)
                {
                    Debug.WriteLine("Key:[{0}], Value:[{1}]", kvp.Key, kvp.Value);
                }
            }
        }

        //Function to write out data to CSV file
        public static void DictToCsv(TweetDict dict, string filePath)
        {
            try
            {
                /*
                var csvLines = String.Join(Environment.NewLine,
                       dict.Select(d => d.Key + "," + d.Value));*/
                var csvLines = String.Format("{{\"id\": \"{0}\",\n\"stamp\": \"{1}\",\n\"text\": \"{2}\"}},\n\n\n", dict.id, dict.stamp, dict.text);
    
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(Directory.GetCurrentDirectory(), "\\" + filePath)));
                File.AppendAllText(filePath, csvLines);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        public List<TweetDict> RecTweetGrabber(DateTime start_date, DateTime end_date)
        {
            string s_date = start_date.ToString("yyyy-MM-dd");
            string e_date = end_date.ToString("yyyy-MM-dd");

            Debug.WriteLine("Searching with start_date:[{0}] end_date:[{1}]", s_date, e_date);

            WebPageGrabber url_grabber = new WebPageGrabber();
            string response = url_grabber.GET(String.Format(base_url, s_date, e_date));


            //Convert response into a List<String> object
            //List<Dictionary<string, string>> response_list = new JavaScriptSerializer().Deserialize<List<Dictionary<string, string>>>(response);
            var response_list = new JavaScriptSerializer().Deserialize<List<TweetDict>>(response);


            // If the List size is 100, which means response limit, try to split in half to get a more accurate dataset that does not meet limit
            if(response_list.Count >= 100)
            {
                Debug.WriteLine("Total List size is:[{0}]", response_list.Count);
                if (start_date == end_date)
                {
                    //Too much data on single day, can't scrape any deeper, Just return the data without splitting
                    return response_list;
                }

                Debug.WriteLine("Splitting Date range: start_date:[{0}] end_date:[{1}]", start_date, end_date);
                // Logic to split Date range in half, make sure left_end and right_start are not same day, to avoid duplicate data
                double half_days = (end_date - start_date).TotalDays / 2;
                //start_date;
                DateTime left_end = start_date.AddDays(half_days);

                DateTime right_start = left_end.AddDays(1);
                //end_date;

                Debug.WriteLine("Left Half is: start_date:[{0}] left_end:[{1}]", start_date, left_end);
                Debug.WriteLine("Right Half is: right_start:[{0}] end_date:[{1}]", right_start, end_date);

                // Recursively Search the smaller date ranges now
                response_list.AddRange(RecTweetGrabber(start_date, left_end));
                response_list.AddRange(RecTweetGrabber(right_start, end_date));

            }

            // If not too much data, automatically return List of JSON string
            return response_list;
        }


    }
}
