using System;
using System.Collections.Generic;
using System.Text;

namespace TestServer.Utils
{
    class PageManager
    {

        private int size;
        private List<Object> list;

        public PageManager(List<Object> list, int size)
        {
            this.size = size;
            this.list = new List<Object>(list.ToArray());
        }

        public PageManager(int size, params Object[] values)
        {
            this.size = size;
            this.list = new List<Object>(values);//Arrays.asList(values);
        }

        public int getTotalPages()
        {
            double v = ((double)this.list.Count / (double)this.size);
            return ((double)(this.list.Count / (double) size)) <= 0 ? 1 : ((v >= double.Parse((int)v + ",1")) ? ((int)v + 1) : ((int)v));
        }

        public string getPage(int page)
        {
            if (page < 1)
                page = 0;
            int size = (page == 0) ? 1 : (this.size * page - this.size);
            List<string> values = new List<string>();
            try
            {
                for (int loop = 0; loop < this.size; ++loop)
                {
                    values.Add(this.list[size + loop] + "");
                }
            }
            catch (Exception ex) { }
            StringBuilder format = new StringBuilder();
            int loop2 = 0;
            int vSize = values.Count;
            while (loop2 < vSize)
            {
                String get = values[loop2];
                ++loop2;
                format.Append(get);
                if (loop2 < vSize)
                    format.Append("<split>");
            }
            return format.ToString();
        }

    }
}
