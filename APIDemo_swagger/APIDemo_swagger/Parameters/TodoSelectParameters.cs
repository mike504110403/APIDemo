using System.Text.RegularExpressions;

namespace APIDemo_swagger.Parameters
{
    public class TodoSelectParameters
    {   
        // 注入server參數
        public string? type { get; set; }
        // 參數化查詢所用類別
        public string? name { get; set; }
        public bool? enable { get; set; } 
        public DateTime? insertTime { get; set; }
        public int? minOrder { get; set; }
        public int? maxOrder { get; set; }

        // 過濾接受值
        private string _order;
        public string? Order
        {
            get { return _order; }
            set
            {
                Regex regex = new Regex(@"^\d*-\d$");
                if (regex.Match(value).Success)
                {
                    minOrder = Int32.Parse(value.Split('-')[0]); // -前
                    maxOrder = Int32.Parse(value.Split('-')[1]); // -後
                }
                _order = value;
            }
        }
        // 過濾接受值
    }
}
