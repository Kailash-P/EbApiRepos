using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SentimentAnalysis
    {
        public int ID { get; set; }

        public string ConsumerName { get; set; }

        public decimal Sentiment { get; set; }

        public string FeedBack { get; set; }

        public string ConsumerProfilePicture { get; set; }
    }
}
