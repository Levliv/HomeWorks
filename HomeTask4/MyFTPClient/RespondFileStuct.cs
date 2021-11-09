namespace MyFTP
{
    public class RespondFileStuct
    {
        public RespondFileStuct(string path_to_file, bool is_dir_flag)
        {
            path = path_to_file;
            is_dir = is_dir_flag;
        }
        public string path;
        public bool is_dir;
    }
}
