using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Badania_Operacyjne___Projekt
{
    public partial class Form1 : Form
    {
        public class User_Input_t
        {
            public string name;
            public List<string> predecessors;
            public float time;                    // liczymy ilość czynności, dodajemy 1 i odejmujemy ilość powtarzających się

            public User_Input_t()
            {
                name = "";
                predecessors = new List<string>();
                time = -1;
            }
            public User_Input_t(string n, List<string> pre, int t)
            {
                name = n;
                predecessors = pre;
                time = t;
            }
        }

        public class Czynnosc_t           // Action
        {
            public List<string> predecessors;


            public string name;
            public float czas_trwania;

            public int node_ind_start;
            public int node_ind_end;

            public Czynnosc_t()
            {
                name = "";
                czas_trwania = -1;

                predecessors = new List<string>();

                node_ind_start = -1;
                node_ind_end = -1;
            }
            public Czynnosc_t(int s, int e)
            {   
                node_ind_start = s;
                node_ind_end = e;
            }
            public Czynnosc_t(string n, float czas, int ind_s, int ind_e)
            {
                name = n;
                czas_trwania = czas;

                node_ind_start = ind_s;
                node_ind_end = ind_e;

                predecessors = null;
            }
        }

        public class Zdarzenie_t           // Event - node
        {
            // czynności, które startują z tego noda
            public List<Czynnosc_t> lista_wychodzacych;

            // czynności, które kończą w tym nodzie
            public List<Czynnosc_t> lista_przychodzacych;

            public float moment_najwczesniejszy;
            public float moment_najpozniejszy;
            public float zapas_czasu;

            public int index;

            public Zdarzenie_t()
            {
                lista_wychodzacych = new List<Czynnosc_t>();
                lista_przychodzacych = new List<Czynnosc_t>();

                moment_najwczesniejszy = -1;
                moment_najpozniejszy = -1;
                zapas_czasu = -1;
                index = -1;
            }
        }

        public class Sciezka_krytyczna
        {
            public List<Zdarzenie_t> zdarzenia_na_sciezce;

            Sciezka_krytyczna()
            {
                zdarzenia_na_sciezce = new List<Zdarzenie_t>();
            }
        }

        public void printt(string s)
        {
            //log.Text += s;            
        }
        public void print(string s)
        {
            //log.Text = s;
            Console.WriteLine(s);
        }

        public Random random = new Random();
        public Form1()
        {
            dir_setup();

            InitializeComponent();
            log.Text = "";

            //var binder = new BindingList<Czynnosc>(list);
            //dataGridView_input_table.DataSource = binder;

            dataGridView_input_table.Columns[0].Width = 100; dataGridView_input_table.Columns[0].ValueType = typeof(String);
            dataGridView_input_table.Columns[1].Width = 100; dataGridView_input_table.Columns[1].ValueType = typeof(String);
            dataGridView_input_table.Columns[2].Width = 100; dataGridView_input_table.Columns[2].ValueType = typeof(String);
            dataGridView_input_table.Columns[3].Width = 100; dataGridView_input_table.Columns[3].ValueType = typeof(String);
            dataGridView_input_table.AllowUserToResizeColumns = false;
            dataGridView_input_table.AllowUserToResizeRows = false;

            for(int i=0; i<4; i++)
            {
                dataGridView_input_table.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView_input_table.Columns[i].CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView_input_table.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            
            pictureBox_legend.Image = Image.FromFile(path_base_legend);
        }



        public String[] acceptable_names_for_activities = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

        public List<User_Input_t> user_input;
        public bool was_the_input_correct = true;
        int round_counter = 0;

        public int how_many_nodesss;
        public Zdarzenie_t[] nodes;
        public Czynnosc_t[] tab_wszystkich_czynnosci;

        List<Zdarzenie_t> critical_path;

        int[,] visualization_tab;
        int size_x = 100; int size_y = 100;
        int visualization_background = -1;
        int mark_around_radious_length = 1;

        int[,] final_visualization_tab;
        int final_v_tab_x;
        int final_v_tab_y;
        int final_width;
        int final_height;

        public Circle[] node_circles;
        int circle_radious = 125;
        int distance_from_the_center_of_circle_start = 12;
        int distance_from_the_center_of_circle_stop = 25;
        float width_of_line = 5;
        Color color_for_critical = Color.Red;
        Color color_for_regular = Color.Black;

        const string dir_fundamentals = @"fundamentals\";
        const string dir_generation_steps = @"generation_steps\";
        const string dir_output = @"output\";

        const string path_base_circle = dir_fundamentals + "base.bmp";
        const string path_base_empty = dir_fundamentals + "empty.bmp";
        const string path_base_legend = dir_fundamentals + "legend.bmp";        

        const string path_first_img = dir_generation_steps + "filled_grid.bmp";        
        const string path_final_img = dir_output + "final.bmp";

        const int final_padding = 75;
        const bool logs = false;
        const bool hard_code = false;

        string path_to_indexed_image(int index)
        {
            return dir_generation_steps + index.ToString() + ".bmp";
        }
        void print_error(in string text)
        {
            Console.WriteLine();
            Console.WriteLine(text);
            Console.WriteLine();
        }
        void make_sure_dir_exists(in string dir)
        {
            try
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            }
            catch
            {
                print_error("failed at creating dir " + dir);
            }            
        }
        void make_sure_dir_is_reset(in string dir)
        {
            try
            {
                if (Directory.Exists(dir)) { Directory.Delete(dir, true); Directory.CreateDirectory(dir); } else { Directory.CreateDirectory(dir); };
            }
            catch
            {
                print_error("failed at reseting dir " + dir);
            }
        }
        void make_sure_file_is_present(in string file)
        {
            if (!File.Exists(file))
            {
                print_error("!!!      program needs " + file + "      !!!");
                PerformOverflow();
            }
        }
        void PerformOverflow()
        {
            PerformOverflow();
        }
        void change_focus()
        {
            this.ActiveControl = label1;
        }
        void dir_setup()
        {
            make_sure_dir_exists(dir_fundamentals);
            make_sure_dir_is_reset(dir_generation_steps);
            make_sure_dir_is_reset(dir_output);

            // crash if project lacks any of this files
            make_sure_file_is_present(path_base_circle);
            make_sure_file_is_present(path_base_empty);
            make_sure_file_is_present(path_base_legend);
        }
        void dir_reset()
        {
            make_sure_dir_is_reset(dir_generation_steps);
            make_sure_dir_is_reset(dir_output);
        }
        

        /////////////////////////////////////////////////////////////////////////////////////////////////
        
        bool only_second_run_shows()
        {
            if (round_counter == 2) return true;
            return false;
        }
        void report_error_input(string text)
        {
            log.Text = text;            
        }
        void reset_error_input()
        {
            log.Text = "";
        }


        bool no_empty_spaces()
        {
            if (dataGridView_input_table.Rows.Count == 1) return false;

            for (int y = 0; y < dataGridView_input_table.Rows.Count - 1; y++)
            {
                var row = dataGridView_input_table.Rows[y];

                for (int x = 0; x < 4; x++)
                {
                    if (row.Cells[x].Value == null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        bool row_2_3_4_can_be_numbers()
        {
            for (int y = 0; y < dataGridView_input_table.Rows.Count - 1; y++)
            {
                var row = dataGridView_input_table.Rows[y];

                Console.WriteLine(row.Cells[1].Value.ToString());
                Console.WriteLine(row.Cells[2].Value.ToString());
                Console.WriteLine(row.Cells[3].Value.ToString());

                if ((!(float.TryParse(row.Cells[1].Value.ToString(), out float f1))) || f1 <= 0) { if (logs) Console.WriteLine("HERE 21"); return false; }
                if ((!int.TryParse(row.Cells[2].Value.ToString(), out int a)) || a <= 0) { if (logs) Console.WriteLine("HERE 22"); return false; }
                if ((!int.TryParse(row.Cells[3].Value.ToString(), out int b)) || b <= 0) { if (logs) Console.WriteLine("HERE 23"); return false; }
            }

            return true;
        }

        bool get_user_input_new_way()
        {
            if (!no_empty_spaces()) { if (logs) Console.WriteLine("HERE 1"); report_error_input("Error input -> you left empty cell"); return false; }
            if (!row_2_3_4_can_be_numbers()) { if (logs) Console.WriteLine("HERE 2"); report_error_input("Error input -> incorrect values"); return false; }

            if(logs) Console.WriteLine("CHECKS PASSED");

            tab_wszystkich_czynnosci = new Czynnosc_t[dataGridView_input_table.Rows.Count - 1];

            int biggest_Do = -1;
            for (int y = 0; y < dataGridView_input_table.Rows.Count - 1; y++)
            {
                var row = dataGridView_input_table.Rows[y];

                string name = row.Cells[0].Value.ToString();
                float czas_trwania =  float.Parse(row.Cells[1].Value.ToString());
                int Od = int.Parse(row.Cells[2].Value.ToString()) - 1;
                int Do = int.Parse(row.Cells[3].Value.ToString()) - 1;
                
                tab_wszystkich_czynnosci[y] = new Czynnosc_t(name, czas_trwania, Od, Do);

                if (biggest_Do < Do) { biggest_Do = Do; }
            }            

            how_many_nodesss = biggest_Do + 1;
            nodes = new Zdarzenie_t[how_many_nodesss]; for (int i = 0; i < how_many_nodesss; i++) { nodes[i] = new Zdarzenie_t(); }

            return true;
        }
        void przypisanie_odpowiednich_czynnosci_do_nodow()
        {
            foreach (var czynnosc in tab_wszystkich_czynnosci)
            {
                if(logs) Console.WriteLine(how_many_nodesss + "   " + czynnosc.name + " " + czynnosc.node_ind_start + " " + czynnosc.node_ind_end);

                // może muszą być co najmniej 2

                nodes[czynnosc.node_ind_start].lista_wychodzacych.Add(czynnosc);

                nodes[czynnosc.node_ind_end].lista_przychodzacych.Add(czynnosc);
            }
        }

        void MAIN_OPERTIONS()
        {
            for (int c = 0; c < 2; c++)
            {
                round_counter++;

                // 1.
                if (hard_code == false) { if (!get_user_input_new_way()) { button1.Text = "save records"; return; } }
                else
                {
                    if ("input 1" == "")
                    {
                        int counter = 0;
                        tab_wszystkich_czynnosci = new Czynnosc_t[8];

                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("A", 5, (1) - 1, (2) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("B", 7, (1) - 1, (3) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("C", 6, (2) - 1, (4) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("D", 8, (2) - 1, (5) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("E", 3, (3) - 1, (5) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("F", 4, (4) - 1, (5) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("G", 2, (4) - 1, (6) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("H", 5, (5) - 1, (6) - 1); counter++;

                        how_many_nodesss = 6;
                        nodes = new Zdarzenie_t[how_many_nodesss]; for (int i = 0; i < how_many_nodesss; i++) { nodes[i] = new Zdarzenie_t(); }
                    }

                    if ("input 2" != "")
                    {
                        int counter = 0;
                        tab_wszystkich_czynnosci = new Czynnosc_t[10];

                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("A", 3, (1) - 1, (2) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("B", 4, (2) - 1, (3) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("C", 6, (2) - 1, (4) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("D", 7, (3) - 1, (5) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("E", 1, (5) - 1, (7) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("F", 2, (4) - 1, (7) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("G", 3, (4) - 1, (6) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("H", 4, (6) - 1, (7) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("I", 1, (7) - 1, (8) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("J", 2, (8) - 1, (9) - 1); counter++;

                        how_many_nodesss = 9;
                        nodes = new Zdarzenie_t[how_many_nodesss]; for (int i = 0; i < how_many_nodesss; i++) { nodes[i] = new Zdarzenie_t(); }
                    }

                    if ("input 3" == "")
                    {
                        int counter = 0;
                        tab_wszystkich_czynnosci = new Czynnosc_t[23];

                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("A", 25, (1) - 1, (2) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("B", 30, (1) - 1, (3) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("C", 50, (1) - 1, (7) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("D", 13, (2) - 1, (4) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("E", 12, (2) - 1, (5) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("F", 19, (3) - 1, (6) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("G", 18, (3) - 1, (8) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("H", 6, (4) - 1, (5) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("I", 8, (4) - 1, (12) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("J", 15, (5) - 1, (9) - 1); counter++;

                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("K", 6, (6) - 1, (7) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("L", 27, (6) - 1, (10) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("M", 19, (6) - 1, (11) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("N", 20, (7) - 1, (9) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("O", 30, (7) - 1, (10) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("P", 20, (8) - 1, (11) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("R", 20, (9) - 1, (12) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("S", 40, (10) - 1, (14) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("T", 6, (11) - 1, (14) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("U", 10, (12) - 1, (13) - 1); counter++;

                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("W", 80, (12) - 1, (15) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("X", 12, (13) - 1, (14) - 1); counter++;
                        tab_wszystkich_czynnosci[counter] = new Czynnosc_t("Y", 50, (14) - 1, (15) - 1); counter++;

                        how_many_nodesss = 15;
                        nodes = new Zdarzenie_t[how_many_nodesss]; for (int i = 0; i < how_many_nodesss; i++) { nodes[i] = new Zdarzenie_t(); }
                    }
                }
                reset_error_input();


                // 2.       Wszystkie czynności są gotowe -> teraz tylko ich przypisanie do właściwych nodów
                przypisanie_odpowiednich_czynnosci_do_nodow();

                // 3.
                kalkulacje_sieci();

                // 4.
                critical_path = wyznaczanie_siezki_krytycznej();

                // 5.

                size_x = how_many_nodesss * 2;
                size_y = how_many_nodesss + 1;

                generating_visualization();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            change_focus();
            dir_reset();            log.Text = "";
            button1.Text = "working on it...";

            {
                MAIN_OPERTIONS();
            }
            
            open_output();
            button1.Text = "save records";
        }
        private void button_output_Click(object sender, EventArgs e)
        {
            open_output();
        }
        void open_output()
        {
            change_focus();
            if (File.Exists(path_final_img))
            {
                Process.Start(path_final_img);
            }           
        }
        private void button_work_dir_Click(object sender, EventArgs e)
        {
            change_focus();
            Process.Start(Directory.GetCurrentDirectory());
        }

        void generating_visualization()
        {
            visualization_tab = new int[size_x, size_y];
            for (int y = 0; y < size_y; y++) for (int x = 0; x < size_x; x++) visualization_tab[x, y] = visualization_background;
            
            mark_around_the_node(0, 0, nodes[0].index); // skoro i tak zawsze ruszamy na prawo, to można zacząć maksymalnie z lewej
            show_visualization_tab();



            analize_layer(0);

            smaller_tab();

            eliminate_repeating_indexes();

            reducing_small_tab();

            empty_spaces_reduction();

            reducing_small_tab();

            pulling_down_indexes();
            
            creating_img_from_visualization_tab();
                        
            adding_arrows();            
        }


        List<pos_index_value> indexes_from_row(int y)
        {
            List<pos_index_value> ret = new List<pos_index_value>();

            for (int x = 0; x < final_v_tab_x; x++)
            {
                int index = final_visualization_tab[x, y] - 1;
                if (index != -1)
                {
                    ret.Add(new pos_index_value(new Point(x, y), index));
                }
            }

            return ret;
        }
        List<connested_indexes> connections_between(List<pos_index_value> list_base_0)
        {
            List<connested_indexes> ret = new List<connested_indexes>();

            for(int i=0; i< list_base_0.Count; i++)
            {
                Zdarzenie_t first = nodes[list_base_0[i].index_value_BASE_0];

                // patrzymy na wszystkie następne na liście
                for (int i_testing = i+1; i_testing < list_base_0.Count; i_testing++)
                {
                    Zdarzenie_t second = nodes[list_base_0[i_testing].index_value_BASE_0];

                    if(are_nodes_connected(first, second))
                    {
                        ret.Add(new connested_indexes(list_base_0[i], list_base_0[i_testing]));
                    }
                }
            }

            return ret;
        }
        bool are_nodes_connected(Zdarzenie_t first, Zdarzenie_t second)
        {
            foreach(var node in first.lista_przychodzacych)
            {
                if (node.node_ind_start == second.index)
                    return true;
            }

            foreach (var node in first.lista_wychodzacych)
            {
                if (node.node_ind_end == second.index)
                    return true;
            }

            return false;
        }
        bool some_in_between_row(Point a, Point b)
        {
            if (a.X + 1 == b.X) return false;

            int y = a.Y;
            for (int x = a.X; x <= b.X; x++)
            {
                if (final_visualization_tab[x, y] != 0) 
                    return true;
            }

            return false;
        }
        void pulling_down_indexes()
        {
            for (int y = 0; y < final_v_tab_y; y++)
            {
                List<pos_index_value> indexes_base0_in_a_row = indexes_from_row(y);

                if (indexes_base0_in_a_row.Count > 1)
                {
                    List<connested_indexes> connected_nodes = connections_between(indexes_base0_in_a_row);

                    foreach(var connection in connected_nodes)
                    {
                        if(some_in_between_row(connection.A.pos, connection.B.pos))
                        {
                            if(random.Next(0, 2) % 2 == 0)
                            {
                                pull_down(connection.A.pos);
                            }
                            else
                            {
                                pull_down(connection.B.pos);
                            }
                        }
                    }
                }
            }
        }

        void pull_down(Point coord_of_value_in_final_tab)
        {
            /*if (only_second_run_shows())
            {
                Console.WriteLine("BEFORE   " + final_visualization_tab[coord_of_value_in_final_tab.X, coord_of_value_in_final_tab.Y]);
                Console.WriteLine("one lower" + final_visualization_tab[coord_of_value_in_final_tab.X, coord_of_value_in_final_tab.Y + 1]);
                Console.WriteLine("");
                show_final_visualization_tab();
            }*/


            add_bottom_row__to_final_tab();

            move_this_row_and_every_lower_ONE_STEP_DOWN(coord_of_value_in_final_tab.Y + 1);

            final_visualization_tab[coord_of_value_in_final_tab.X, coord_of_value_in_final_tab.Y + 1] = final_visualization_tab[coord_of_value_in_final_tab.X, coord_of_value_in_final_tab.Y];
            final_visualization_tab[coord_of_value_in_final_tab.X, coord_of_value_in_final_tab.Y] = 0;


            /*if (only_second_run_shows())
            {
                Console.WriteLine("AFTER");
                Console.WriteLine("");
                show_final_visualization_tab();
            }*/
        }
        void add_bottom_row__to_final_tab()
        {
            int[,] buffer = new int[final_v_tab_x, (final_v_tab_y + 1)];

            for (int y = 0; y < final_v_tab_y + 1; y++)
            {
                for (int x = 0; x < final_v_tab_x; x++)
                {
                    buffer[x, y] = 0;
                }
            }
            for (int y = 0; y < final_v_tab_y; y++)
            {
                for (int x = 0; x < final_v_tab_x; x++)
                {
                    buffer[x, y] = final_visualization_tab[x, y];
                }
            }

            // CHANGE
            final_v_tab_y += 1;
            final_visualization_tab = new int[final_v_tab_x, final_v_tab_y];

            for (int y = 0; y < final_v_tab_y; y++)
            {
                for (int x = 0; x < final_v_tab_x; x++)
                {
                    final_visualization_tab[x, y] = buffer[x, y];
                }
            }
        }


        void move_this_row_and_every_lower_ONE_STEP_DOWN(int y_of_row)
        {
            for (int y = final_v_tab_y - 1; y > y_of_row; y--)
            {
                for (int x = 0; x < final_v_tab_x; x++)
                {
                    final_visualization_tab[x, y] = final_visualization_tab[x, y-1];
                }
            }

            for (int x = 0; x < final_v_tab_x; x++)
                final_visualization_tab[x, y_of_row] = 0;
        }


        void empty_spaces_reduction()
        {
            for (int x = 0; x < final_v_tab_x; x++)
            {
                if(check_if_column_is_empty(x))
                {
                    for(int x_testing = x + 1; x_testing < final_v_tab_x; x_testing++)
                    {
                        if(!check_if_column_is_empty(x_testing))
                        {
                            swap_columns_first_is_filled_with_second(x, x_testing);
                        }
                    }
                }
            }
        }
        void swap_columns_first_is_filled_with_second(int x1, int x2)
        {
            for (int y = 0; y < final_v_tab_y; y++)
            {
                var tmp = final_visualization_tab[x1, y];
                final_visualization_tab[x1, y] = final_visualization_tab[x2, y];
                final_visualization_tab[x2, y] = tmp;
            }
        }
        bool check_if_column_is_empty(int x)
        {
            for (int y = 0; y < final_v_tab_y; y++)
            {
                if (final_visualization_tab[x, y] != 0)
                {
                    return false;
                }
            }
            
            return true;
        }


        void filling_node_circles()
        {
            node_circles = new Circle[how_many_nodesss];

            for (int y = 0; y < final_v_tab_y; y++)
                for (int x = 0; x < final_v_tab_x; x++)
                {
                    int index = final_visualization_tab[x, y] - 1;

                    int circle_x = (x * final_width) + final_padding + circle_radious;
                    int circle_y = (y * final_height) + final_padding + circle_radious;

                    if (index != -1)
                    {
                        node_circles[index] = new Circle( new Point(circle_x, circle_y), (float)circle_radious);
                    }
                }
        }

        bool is_equal(Zdarzenie_t a, Zdarzenie_t b)
        {
            if((a.index == b.index) && (a.moment_najpozniejszy == b.moment_najpozniejszy) && (a.moment_najwczesniejszy == b.moment_najwczesniejszy))
            {
                return true;
            }

            return false;
        }
        Czynnosc_t find_corresponding_Czynnosc(Zdarzenie_t a, Zdarzenie_t b)
        {
            foreach (var czynnosc in tab_wszystkich_czynnosci)
            {
                if (czynnosc.node_ind_start == a.index && czynnosc.node_ind_end == b.index)
                {
                    //Console.WriteLine("found one   ");
                    //Console.WriteLine(a.index + " " + b.index);
                    //Console.WriteLine(czynnosc.name);

                    return czynnosc;
                }
            }

            return null;
        }
        (bool, Czynnosc_t) is_this_critical(Zdarzenie_t a, Zdarzenie_t b)
        {
            var corresponding_Czynnosc = find_corresponding_Czynnosc(a, b);

            for (int i=1; i < critical_path.Count; i++)
            {
                if(is_equal(critical_path[i - 1], a) && is_equal(critical_path[i], b))
                {
                    return (true, corresponding_Czynnosc);
                }
            }
            return (false, corresponding_Czynnosc);
        }
        void adding_arrows()
        {
            filling_node_circles();

            Bitmap final_image = new Bitmap(path_first_img);
            Graphics g = Graphics.FromImage(final_image);

            for (int y = 0; y < final_v_tab_y; y++) for (int x = 0; x < final_v_tab_x; x++)
                {
                    int index = final_visualization_tab[x, y] - 1;

                    if (index != -1)
                    {
                        foreach(var linia in nodes[index].lista_wychodzacych)
                        {
                            int other_index = linia.node_ind_end;

                            Color color;
                            var answer = is_this_critical(nodes[index], nodes[other_index]);
                            if (answer.Item1)
                                color = color_for_critical;
                            else
                                color = color_for_regular;

                            string description = answer.Item2.name + " - " + answer.Item2.czas_trwania.ToString();

                            line_between(ref g, node_circles[index], node_circles[other_index], circle_radious + distance_from_the_center_of_circle_start, circle_radious + distance_from_the_center_of_circle_stop,
                                            color, width_of_line, description);
                        }
                    }
                }
            g.Dispose();

            final_image.Save(path_final_img);
            final_image.Dispose();
        }
        (Point, Point) line_markers(Circle A, Circle B, float radious_A, float radious_B)
        {
            return (distance_between_circles(A, B, radious_A), distance_between_circles(B, A, radious_B));
        }
        Point distance_between_circles(Circle A, Circle B, float radious)
        {
            vector v = new vector(A, B);

            float t = (float)Math.Sqrt((Math.Pow(radious, 2)) / (Math.Pow(v.x, 2) + Math.Pow(v.y, 2)));

            Point p_with_plus = new Point((int)(A.center.X + v.x * t), (int)(A.center.Y + v.y * t));
            Point p_with_minus = new Point((int)(A.center.X - v.x * t), (int)(A.center.Y - v.y * t));

            vector with_plus = new vector(p_with_plus, B.center);
            vector with_minus = new vector(p_with_minus, B.center);

            // which one is closer to B
            if (with_plus.distance < with_minus.distance)
                return p_with_plus;

            return p_with_minus;
        }

        void adding_arrow_graphically(ref Graphics g, Color color, PointF start, PointF end, Pen line_pen, Pen arrow_pen, float arrowSize, bool filled, string text)
        {
            if (start == end) return;

            PointF mid = new PointF((start.X + end.X) / 2, (start.Y + end.Y) / 2);
            float angle = (float)(180 / Math.PI * Math.Atan2(end.Y - start.Y, end.X - start.X));
            float angle_2 = (float)(180 / Math.PI * Math.Atan2(start.Y - end.Y, start.X - end.X));

            // Console.WriteLine(text + "  angle -> " + angle);

            float angle_for_arrows = angle;
            float angle_for_text;

            int direction = (int)(end.X - start.X);


            // Point size = new Point(120, 50);

            if (direction > 0)
            {
                // TO THE RIGHT
                var answer = line_markers(new Circle(start, circle_radious), new Circle(mid, circle_radious), 0, 60);
                mid = answer.Item2;

                angle_for_text = angle;
            }
            else if (direction < 0)
            {
                // TO THE LEFT
                var answer = line_markers(new Circle(end, circle_radious), new Circle(mid, circle_radious), 0, 50);
                mid = answer.Item2;

                angle_for_text = angle_2;
            }
            else
            {
                // DOWN
                var answer = line_markers(new Circle(start, circle_radious), new Circle(mid, circle_radious), 0, 60);
                mid = answer.Item2;

                angle_for_text = angle;
            }


            var gp = new GraphicsPath();
            gp.AddLines(
                new PointF[]
                {
                new PointF(-arrowSize, -arrowSize/3),
                new PointF(0, 0),
                new PointF(-arrowSize, arrowSize/3)
                }
            );
            if (filled)
            {
                gp.CloseFigure();
            }
            var state = g.Save();
            
            {
                g.DrawLine(line_pen, start, end);
                g.TranslateTransform(
                    end.X,
                    end.Y);
                g.RotateTransform(angle_for_arrows);
                if (filled)
                {
                    using (Brush fill = new SolidBrush(color))
                    {
                        g.FillPath(fill, gp);
                    }
                }
                g.DrawPath(arrow_pen, gp);

            }
            g.Restore(state);

            add_line_description(ref g, text, mid, angle_for_text);
        }
        void add_line_description(ref Graphics g, string description, PointF pos, float angle)
        {
            var state = g.Save();
            g.ResetTransform();
            {
                Point size = new Point(120, 50);

                var rectf = new RectangleF(0, 0, size.X, size.Y); //rectf for My Text
                //var rectf = new RectangleF(pos.X, pos.Y, size.X, size.Y); //rectf for My Text

                if (angle > 90) angle = 90 - angle;

                if(logs) Console.WriteLine(description + "   " + angle);

                g.RotateTransform(angle);
                g.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);

                //float r = 100; rectf.X = r; rectf.Y = r;
                //rectf.X = pos.X;
                //rectf.Y = pos.Y;
                
                //g.DrawRectangle(new Pen(Color.Red, 2), pos.X, pos.Y, size.X, size.Y);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                g.DrawString(description.ToString(), new System.Drawing.Font("Tahoma", 20, FontStyle.Bold), Brushes.Black, rectf, sf);
            }
            g.Restore(state);
        }
        void line_between(ref Graphics g, Point a, Point b, Color c, float width, string text)
        {
            using (Pen line_pen = new Pen(c, width))
            using (Pen arrow_pen = new Pen(c, 10))
            {
                adding_arrow_graphically(ref g, c, a, b, line_pen, arrow_pen, 10f, true, text);
            }   
        }
        void line_between(ref Graphics g, Circle A, Circle B, float radious_A, float radious_B, Color c, float width, string text)
        {
            var (a, b) = line_markers(A, B, radious_A, radious_B);

            line_between(ref g, a, b, c, width, text);
        }



        // Image processing
        void add_padding(ref Bitmap img, string name)
        {
            final_width = img.Width + 2 * final_padding;
            final_height = img.Height + 2 * final_padding;

            Bitmap with_padding = new Bitmap(final_width, final_height); using (Graphics gfx = Graphics.FromImage(with_padding)) using (SolidBrush brush = new SolidBrush(Color.White)) gfx.FillRectangle(brush, 0, 0, final_width, final_height);

            Graphics g_ = Graphics.FromImage(with_padding);
            {
                g_.DrawImage(img, new Point(final_padding, final_padding));
                with_padding.Save(name);
                g_.Dispose();
            }
            img.Dispose();
            with_padding.Dispose();
        }
        void empty_with_padding()
        {
            if(logs) Console.WriteLine(path_base_empty);
            Bitmap empty_with_padding = new Bitmap(path_base_empty);
            add_padding(ref empty_with_padding, path_to_indexed_image(0));
        }
        void creating_img_from_visualization_tab()
        {
            empty_with_padding();

            for (int i=0; i<how_many_nodesss; i++)
                generate_single_cell(nodes[i]);
            
            
            final_picture_applying_img_to_cells();
        }

        int smallest_width()
        {
            int big_width = 0;

            for (int y = 0; y < final_v_tab_y; y++)
                for (int x = 0; x < final_v_tab_x; x++)
                {
                    if (final_visualization_tab[x, y] != 0)
                    {
                        // Console.WriteLine("found " + final_visualization_tab[x, y]);
                        if (x > big_width)
                        {
                            big_width = x;
                            // Console.WriteLine("updating to bigger " + big_width);
                        }
                    }
                }

            return big_width;
        }
        int smallest_height()
        {
            int big_height = 0;

            for (int x = 0; x < final_v_tab_x; x++)
                for (int y = 0; y < final_v_tab_y; y++)
                {
                    if (final_visualization_tab[x, y] != 0)
                    {
                        if (y > big_height)
                        {
                            big_height = y;
                        }
                    }
                }

            return big_height;
        }

        int biggest_width(int end_marker)
        {
            int big_width = 0;

            for (int y = 0; y < size_y; y++)
            for (int x = 0; x < size_x; x++)
                {
                    if(visualization_tab[x, y] == end_marker)
                    {
                        if (x > big_width)
                        {
                            big_width = x;
                        }
                        break;
                    }                    
                }

            return big_width;
        }
        int biggest_height(int end_marker)
        {
            int big_height = 0;

            for (int x = 0; x < size_x; x++)
            for (int y = 0; y < size_y; y++)
                {
                    if (visualization_tab[x, y] == end_marker)
                    {
                        if (y > big_height)
                        {
                            big_height = y;
                        }
                        break;
                    }
                }

            return big_height;
        }
        void smaller_tab()
        {
            final_v_tab_x = biggest_width(visualization_background) - 1;
            final_v_tab_y = biggest_height(visualization_background) - 1;

            final_visualization_tab = new int[final_v_tab_x, final_v_tab_y];

            for (int y = 0; y < final_v_tab_y; y++)
            {
                for (int x = 0; x < final_v_tab_x; x++)
                {
                    if (visualization_tab[x, y] != -1)
                        final_visualization_tab[x, y] = visualization_tab[x, y];
                    else
                        final_visualization_tab[x, y] = 0;
                }
            }            
        }
        void reducing_small_tab()
        {
            int[,] buffer = new int[final_v_tab_x, final_v_tab_y];

            for (int y = 0; y < final_v_tab_y; y++)
            {
                for (int x = 0; x < final_v_tab_x; x++)
                {
                    buffer[x, y] = final_visualization_tab[x, y];
                }
            }

            final_v_tab_x = smallest_width() + 1;
            final_v_tab_y = smallest_height() + 1;

            final_visualization_tab = new int[final_v_tab_x, final_v_tab_y];

            for (int y = 0; y < final_v_tab_y; y++)
            {
                for (int x = 0; x < final_v_tab_x; x++)
                {
                    final_visualization_tab[x, y] = buffer[x, y];
                }
            }
        }
        Bitmap img_of_node(int index)
        {
            return (Bitmap) Bitmap.FromFile(path_to_indexed_image(index));
        }
        void final_picture_applying_img_to_cells()
        {
            Bitmap final_image = new Bitmap(final_width * final_v_tab_x, final_height * final_v_tab_y);
            Graphics g = Graphics.FromImage(final_image);

            for (int y = 0; y < final_v_tab_y; y++) for (int x = 0; x < final_v_tab_x; x++)
                {
                    Bitmap current = img_of_node(final_visualization_tab[x, y]);
                    g.DrawImage(current, new Point(x * final_width, y * final_height));
                    current.Dispose();
                }

            g.Dispose();
            final_image.Save(path_first_img);
        }

        void generate_single_cell(Zdarzenie_t node)
        {
            Bitmap myBitmap = new Bitmap(path_base_circle);

            int index = node.index + 1;
            Graphics g = Graphics.FromImage(myBitmap);
            {
                fill_up(ref g, index);
                fill_down(ref g, (int)node.zapas_czasu);
                fill_left(ref g, (int)node.moment_najwczesniejszy);
                fill_right(ref g, (int)node.moment_najpozniejszy);
                g.Dispose();
            }

            add_padding(ref myBitmap, path_to_indexed_image(index));
        }
        void fill_up(ref Graphics g, int value)
        {
            Point pos = new Point(88, 32);
            Point size = new Point(75, 50);

            var rectf = new RectangleF(pos.X, pos.Y, size.X, size.Y); //rectf for My Text
            //g.DrawRectangle(new Pen(Color.Red, 2), pos.X, pos.Y, size.X, size.Y);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            
            g.DrawString(value.ToString(), new System.Drawing.Font("Tahoma", 24, FontStyle.Bold), Brushes.Black, rectf, sf);
        }
        void fill_down(ref Graphics g, int value)
        {
            Point pos = new Point(88, 175);
            Point size = new Point(75, 50);

            var rectf = new RectangleF(pos.X, pos.Y, size.X, size.Y); //rectf for My Text
            //g.DrawRectangle(new Pen(Color.Red, 2), pos.X, pos.Y, size.X, size.Y);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            g.DrawString(value.ToString(), new System.Drawing.Font("Tahoma", 24, FontStyle.Bold), Brushes.Black, rectf, sf);
        }
        void fill_left(ref Graphics g, int value)
        {
            Point pos = new Point(18, 105);
            Point size = new Point(75, 50);

            var rectf = new RectangleF(pos.X, pos.Y, size.X, size.Y); //rectf for My Text
            //g.DrawRectangle(new Pen(Color.Red, 2), pos.X, pos.Y, size.X, size.Y);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            g.DrawString(value.ToString(), new System.Drawing.Font("Tahoma", 24, FontStyle.Bold), Brushes.Black, rectf, sf);
        }
        void fill_right(ref Graphics g, int value)
        {
            Point pos = new Point(160, 105);
            Point size = new Point(75, 50);

            var rectf = new RectangleF(pos.X, pos.Y, size.X, size.Y); //rectf for My Text
            //g.DrawRectangle(new Pen(Color.Red, 2), pos.X, pos.Y, size.X, size.Y);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            g.DrawString(value.ToString(), new System.Drawing.Font("Tahoma", 24, FontStyle.Bold), Brushes.Black, rectf, sf);
        }












        // OPERACJE NA MACIERZY
        void analize_layer(int y_depth)
        {
            if (logs) Console.WriteLine("func: analize_layer  is CALLED");

            // tutaj można pod koniec wywołać jeszcze raz jeśli któryś z dzieci ma coś w liście wychodzącej

            // 1. find parent
            // 2. mark its childen (move if necessary)
            // 3. go to next parent (stop at -1)

            for(int x=0; visualization_tab[x, y_depth] != -1; x++)
            {
                if(visualization_tab[x, y_depth] != 0 && visualization_tab[x, y_depth] != -1)
                {
                    //Console.WriteLine("Marking children for " + x + " " + y_depth);
                    mark_children(ref x, y_depth);
                }
            }
            show_visualization_tab();

            if (logs) Console.WriteLine("Layer analized");

            if (some_parent_have_children(y_depth + 1))
                analize_layer(y_depth + 1);
        }
        void mark_children(ref int parent_x, int parent_y_depth)
        {
            int parent_index = visualization_tab[parent_x, parent_y_depth] - 1;
            int children_y = parent_y_depth + 1;

            //Console.WriteLine("Marking children for parent " + parent_index);

            if (nodes[parent_index].lista_wychodzacych.Count() == 0)
            {
                //Console.WriteLine("parent " + parent_index + " does not have children");
            }
            else if(nodes[parent_index].lista_wychodzacych.Count() == 1)
            {
                //Console.WriteLine("parent " + parent_index + " has 1 children");

                mark_around_the_node(parent_x, children_y, nodes[parent_index].lista_wychodzacych[0].node_ind_end);
            }
            else
            {
                //Console.WriteLine("parent " + parent_index + " has many children");

                int pos_difference = 0;

                // pierwszy równo pod
                mark_around_the_node(parent_x + pos_difference, children_y, nodes[parent_index].lista_wychodzacych[0].node_ind_end);


                for (int i = 1; i < nodes[parent_index].lista_wychodzacych.Count(); i++)
                {
                    // i jest indexem listy parenta

                    int children_node_index = nodes[parent_index].lista_wychodzacych[i].node_ind_end;
                    int children_x;

                    Chunk_dimensions chunk = new Chunk_dimensions();
                    chunk.y_start = 0;
                    chunk.y_stop = children_y;
                    
                    if(i % 2 == 1)  // na prawo
                    {
                        pos_difference = (i + 1) / 2;
                        children_x = parent_x + pos_difference;
                    }
                    else            // na lewo
                    {
                        pos_difference = -i / 2;
                        children_x = parent_x + pos_difference + 1;

                        parent_x++;
                    }

                    chunk.x_start = children_x;
                    chunk.x_stop = length_x_of_layer(children_y);


                    chunk.calc_lengths();
                    move_chunk(chunk, 1, 0);

                    // move_chunk(); // na prawo (dla różnych i < 0 lub i > 0, będzie inaczej się zaczynął początek chunk'u)

                    // po move -> będzie zmiana współrzędnych

                    mark_around_the_node(children_x, children_y, children_node_index);

                    show_visualization_tab();
                }

                if(logs) Console.WriteLine("Marking stopped");
            }
        }
        //                                              [y - 1 (up)]    [y + 1 (down)]    [x - 1 (left)]    [x + 1 (right)]
        void move_chunk(Chunk_dimensions chunk, int vec_x, int vec_y)
        {
            if(logs) Console.WriteLine("Moving chunk         x: " + chunk.x_start + " " + chunk.x_stop + "  y: " + chunk.y_start + " " + chunk.y_stop);

            // zmiana z ruszaniem chunk --> to nie może być cała tablica !!!
            
            int[,] buffer = new int[chunk.x_length + 1, chunk.y_length + 1];

            if (logs) Console.WriteLine("chunk size   " + chunk.x_length + " " + chunk.y_length);

            // buffer fill up
            for (int y = 0; y <= chunk.y_length; y++) for (int x = 0; x <= chunk.x_length; x++)
                {
                    int tab_x = x + chunk.x_start;
                    int tab_y = y + chunk.y_start;

                    buffer[x, y] = visualization_tab[tab_x, tab_y];
                    visualization_tab[tab_x, tab_y] = 0;
                }

            // buffer to real tab
            for (int y = 0; y <= chunk.y_length; y++) for (int x = 0; x <= chunk.x_length; x++)
                {
                    int tab_x = x + chunk.x_start + vec_x;
                    int tab_y = y + chunk.y_start + vec_y;

                    //Console.WriteLine("tab   " + tab_x + " " + tab_y);
                    //Console.WriteLine("chunk " + x + " " + y);
                    //Console.WriteLine();

                    visualization_tab[tab_x, tab_y] = buffer[x, y];
                }

            {
                /*if (false)
                {
                    //int[,] buffer = new int[size_x, size_y];

                    // to buffer and reset in original
                    for (int y = chunk.y_start; y <= chunk.y_stop; y++) for (int x = chunk.x_start; x <= chunk.x_stop; x++)
                        {
                            buffer[x, y] = visualization_tab[x, y];
                            //visualization_tab[x, y] = 0;
                        }

                    for (int y = chunk.y_start; y <= chunk.y_stop; y++) for (int x = chunk.x_start; x <= chunk.x_stop; x++)
                        {
                            if (x + vec_x < size_x && y + vec_y < 0)
                                visualization_tab[x + vec_x, y + vec_y] = buffer[x, y];
                        }
                }*/
            }
        }

        bool some_parent_have_children(int y_depth)
        {
            for (int x = 0; visualization_tab[x, y_depth] != -1; x++)
            {
                if (visualization_tab[x, y_depth] != 0 && visualization_tab[x, y_depth] != -1)
                {
                    int index_of_node = visualization_tab[x, y_depth] - 1;

                    if (nodes[index_of_node].lista_wychodzacych.Count > 0)
                        return true;
                }
            }

            return false;
        }
        int length_x_of_layer(int y)
        {
            for(int x = 0; x < size_x; x++)
            {
                if (visualization_tab[x, y] == visualization_background)
                    return x;
            }
            return -1;
        }
        void mark_around_the_node(int x, int y, int incoming_index)
        {
            visualization_tab[x, y] = incoming_index + 1;
            zeros_around(x, y);
        }
        void zeros_around(int real_x, int real_y)
        {
            for (int y = max(real_y - mark_around_radious_length, 0); y <= min(real_y + mark_around_radious_length, size_y - 1); y++)
            {
                for (int x = max(real_x - mark_around_radious_length, 0); x <= min(real_x + mark_around_radious_length, size_x - 1); x++)
                {
                    if (visualization_tab[x, y] == -1)
                        visualization_tab[x, y] = 0;
                }
            }
        }

        

        void eliminate_repeating_indexes()
        {
            List<Circle>[] lista_kółek_do_sprawdzenia_przecieć = new List<Circle>[how_many_nodesss];    for (int i = 0; i < how_many_nodesss; i++) lista_kółek_do_sprawdzenia_przecieć[i] = new List<Circle>();
            List<Point>[] coords_of_indexies_in_tab = new List<Point>[how_many_nodesss];                for (int i = 0; i < how_many_nodesss; i++) coords_of_indexies_in_tab[i] = new List<Point>();
            for (int y = 0; y < final_v_tab_y; y++) for (int x = 0; x < final_v_tab_x; x++)
                {
                    int current_index = final_visualization_tab[x, y] - 1;

                    if (current_index != -1)
                    {
                        coords_of_indexies_in_tab[current_index].Add(new Point(x, y));

                        int circle_x = (x * final_width) + final_padding + circle_radious;
                        int circle_y = (y * final_height) + final_padding + circle_radious;

                        lista_kółek_do_sprawdzenia_przecieć[current_index].Add(new Circle(new Point(circle_x, circle_y), circle_radious));
                    }
                }

            int buffer_value = 0;
            Point po = new Point(-1, -1);
            for (int i = 0; i < how_many_nodesss; i++)
            {
                List<Point> available_tab_spots_for_index = coords_of_indexies_in_tab[i];
                List<Circle> available_circles_for_index = lista_kółek_do_sprawdzenia_przecieć[i];

                if (available_tab_spots_for_index.Count > 1)
                {
                    value_and_eliminate(ref buffer_value, ref available_tab_spots_for_index);

                    po = chosen_coord_from_available_spots(i, ref coords_of_indexies_in_tab, ref lista_kółek_do_sprawdzenia_przecieć);
                    
                    final_visualization_tab[po.X, po.Y] = buffer_value;
                }
            }
        }
        Point chosen_coord_from_available_spots(int current_index, ref List<Point>[] coords_of_indexies_in_tab, ref List<Circle>[] lista_kółek_do_sprawdzenia_przecieć)
        {
            // po dokonaniu wyboru usuwany z listy inne opcje dla tego indexu

            Zdarzenie_t current = nodes[current_index];
            int[] ile_przeciec_ma_ten_mozliwy_koniec = new int[lista_kółek_do_sprawdzenia_przecieć[current_index].Count]; for (int i = 0; i < lista_kółek_do_sprawdzenia_przecieć[current_index].Count; i++) { ile_przeciec_ma_ten_mozliwy_koniec[i] = 0; }



            for (int i=0; i < lista_kółek_do_sprawdzenia_przecieć[current_index].Count; i++)                 // przeszukujemy tu - możliwe miejsca
            {
                Circle mozliwy_koniec = lista_kółek_do_sprawdzenia_przecieć[current_index][i];

                foreach(var przychodzacy in current.lista_przychodzacych)
                {
                    int index_stalego__poczatka_linii = przychodzacy.node_ind_start;
                    Circle poczatek = lista_kółek_do_sprawdzenia_przecieć[index_stalego__poczatka_linii][0];        // iterujemy po tym - stałe początki

                    var Actual_points = line_markers(poczatek, mozliwy_koniec, circle_radious + distance_from_the_center_of_circle_start, circle_radious + distance_from_the_center_of_circle_stop);


                    // teraz sprawdzamy czy ta linia przecina aktualne kółko z listy przychodzących, jak przecina, to dostaje + 1
                    if(check_if_line_intersects_with_ANY_other__circles(i, Actual_points, ref lista_kółek_do_sprawdzenia_przecieć))
                    {
                        ile_przeciec_ma_ten_mozliwy_koniec[i]++;
                    }
                }
            }

            // ZNAJDUJEMY możliwe miejsca

            List<Point> all_possible_places = new List<Point>();
            int looking_for_this_many_intersections = -1;
            do
            {
                looking_for_this_many_intersections++;

                for(int i = 0; i < lista_kółek_do_sprawdzenia_przecieć[current_index].Count; i++)
                {
                    if (ile_przeciec_ma_ten_mozliwy_koniec[i] == looking_for_this_many_intersections)
                    {
                        var p = coords_of_indexies_in_tab[current_index][i];

                        all_possible_places.Add(new Point(p.X, p.Y));
                    }
                }
            }
            while (all_possible_places.Count == 0);

            Point chosen_one = all_possible_places[random.Next(0, all_possible_places.Count())];

            int circle_x = (chosen_one.X * final_width) + final_padding + circle_radious;
            int circle_y = (chosen_one.Y * final_height) + final_padding + circle_radious;


            coords_of_indexies_in_tab[current_index].Clear();
            lista_kółek_do_sprawdzenia_przecieć[current_index].Clear();

            coords_of_indexies_in_tab[current_index].Add(chosen_one);
            lista_kółek_do_sprawdzenia_przecieć[current_index].Add(new Circle(new Point(circle_x, circle_y), circle_radious));


            return chosen_one;
        }
        bool check_if_line_intersects_with_ANY_other__circles(int current_index, (Point, Point) actual_points, ref List<Circle>[] lista_kółek_do_sprawdzenia_przecieć)
        {
                                                // iterujemy po wcześniejszych kółkach
            for(int i=0; i<current_index; i++)
            {
                Circle current_kolko = lista_kółek_do_sprawdzenia_przecieć[i][0];

                if (lista_kółek_do_sprawdzenia_przecieć[i].Count > 1 || lista_kółek_do_sprawdzenia_przecieć[i].Count == 0)
                {
                    Console.WriteLine("check_if_line_intersects_with_other__static_circles -     if (lista_kółek_do_sprawdzenia_przecieć[i].Count > 1)");
                    PerformOverflow();
                }
                else
                {
                    double a, b, c, delta;

                    Point C = new Point((int)current_kolko.center.X, (int)current_kolko.center.Y);
                    Point o = actual_points.Item1;
                    vector v = new vector(actual_points.Item1, actual_points.Item2);
                    double distance_to_center = (double)circle_radious;

                    a = (Math.Pow(v.x, 2) + Math.Pow(v.y, 2));
                    b = 2 * ((v.x * o.X) - (C.X * v.x) + (v.y * o.Y) - (C.Y * v.y));
                    c = 
                        Math.Pow(o.X, 2) - 2 * (C.X * o.X) + Math.Pow(C.X, 2) +
                        Math.Pow(o.Y, 2) - 2 * (C.Y * o.Y) + Math.Pow(C.Y, 2) -
                        distance_to_center;

                    delta = Math.Pow(b, 2) + 4 * (a * c);

                    if (delta >= 0)     // intersection happens
                        return true;
                }
            }

            return false;
        }
        
        void value_and_eliminate(ref int buffer_value, ref List<Point> current)
        {
            buffer_value = final_visualization_tab[current[0].X, current[0].Y];
            // eliminating all
            foreach (var p in current)
                final_visualization_tab[p.X, p.Y] = 0;
        }























































        int max(int a, int b)
        {
            if (a > b) return a;
            return b;
        }
        int min(int a, int b)
        {
            if (a < b) return a;
            return b;
        }
        void show_visualization_tab()
        {
            if (logs)
            {
                for (int y = 0; y < size_y; y++)
                {
                    for (int x = 0; x < size_x; x++)
                    {
                        Console.Write(visualization_tab[x, y] + " ");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine(); Console.WriteLine();
            }
        }
        void show_final_visualization_tab()
        {   
            {
                for (int y = 0; y < final_v_tab_y; y++)
                {
                    for (int x = 0; x < final_v_tab_x; x++)
                    {
                        Console.Write(final_visualization_tab[x, y] + " ");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine(); Console.WriteLine();
            }
        }        
        private void kalkulacje_sieci()
        {
            int top = 0;
            int bottom = how_many_nodesss - 1;

            nodes[top].moment_najwczesniejszy = 0;

            // top_down  ->  moment_najwczesniejszy
            {
                while(nodes[bottom].moment_najwczesniejszy == -1)
                {
                    top_down_connections();
                }
            }

            nodes[bottom].moment_najpozniejszy = nodes[bottom].moment_najwczesniejszy;

            // bottom_up  ->  moment_najpozniejszy
            {
                while (nodes[top].moment_najpozniejszy == -1)
                {
                    bottom_up_connections();
                }
            }
        }



        private bool top_down_wszystkie_czynnosci_wychodza_ze_zdarzen_ktore_posiadaja_wartosc(List<Czynnosc_t> lista_przychodzacych)
        {
            foreach(var czynnosc in lista_przychodzacych)
            {
                Zdarzenie_t zdarzenie_z_ktorego_czynnosc_zaczyna = nodes[czynnosc.node_ind_start];

                if(zdarzenie_z_ktorego_czynnosc_zaczyna.moment_najwczesniejszy == -1) return false;
            }

            return true;
        }
        private float top_down_biggest_time(List<Czynnosc_t> lista_przychodzacych)
        {
            float max = -1;
            foreach (var czynnosc_przychodzaca in lista_przychodzacych)
            {
                float val = czynnosc_przychodzaca.czas_trwania + nodes[czynnosc_przychodzaca.node_ind_start].moment_najwczesniejszy;

                if (val > max) max = val;
            }

            return max;
        }
        private void top_down_connections()
        {
            foreach(var node in nodes)
            {
                if((node.moment_najwczesniejszy == -1) && (node.lista_przychodzacych.Count >= 1))
                {
                    if(top_down_wszystkie_czynnosci_wychodza_ze_zdarzen_ktore_posiadaja_wartosc(node.lista_przychodzacych))
                    {
                        node.moment_najwczesniejszy = top_down_biggest_time(node.lista_przychodzacych);
                    }
                }
            }
        }



        private bool bottom_up_wszystkie_czynnosci_wychodza_ze_zdarzen_ktore_posiadaja_wartosc(List<Czynnosc_t> lista_wychodzacych)
        {
            foreach (var czynnosc in lista_wychodzacych)
            {
                Zdarzenie_t zdarzenie_na_ktorym_czynnosc_konczy = nodes[czynnosc.node_ind_end];

                if (zdarzenie_na_ktorym_czynnosc_konczy.moment_najpozniejszy == -1) return false;
            }

            return true;
        }
        private float bottom_up_smallest_time(List<Czynnosc_t> lista_wychodzacych)
        {
            float mini = 1000000000;
            foreach (var czynnosc_wychodzaca in lista_wychodzacych)
            {
                float val = nodes[czynnosc_wychodzaca.node_ind_end].moment_najpozniejszy - czynnosc_wychodzaca.czas_trwania;

                if (val < mini) mini = val;
            }

            return mini;
        }
        private void bottom_up_connections()
        {
            foreach (var node in nodes)
            {
                if ((node.moment_najpozniejszy == -1) && (node.lista_wychodzacych.Count >= 1))
                {
                    if (bottom_up_wszystkie_czynnosci_wychodza_ze_zdarzen_ktore_posiadaja_wartosc(node.lista_wychodzacych))
                    {
                        node.moment_najpozniejszy = bottom_up_smallest_time(node.lista_wychodzacych);
                    }
                }
            }
        }


        private void wyliczanie_zapasu_czasu()
        {
            foreach (var node in nodes)
            {
                node.zapas_czasu = node.moment_najpozniejszy - node.moment_najwczesniejszy;
            }
        }
        private void path_detected(ref List<List<Zdarzenie_t>> mozliwe_sciezki, List<Zdarzenie_t> wczesniejsza_sciezka)
        {
            Zdarzenie_t last_element_in_the_list = wczesniejsza_sciezka[wczesniejsza_sciezka.Count - 1];

            if (last_element_in_the_list.lista_wychodzacych.Count == 0) // koniec -> dotarliśmy do ostatniego noda
            {
                mozliwe_sciezki.Add(wczesniejsza_sciezka);
            }
            else
            {
                foreach (var lista_wychodzacych in last_element_in_the_list.lista_wychodzacych)
                {
                    if (nodes[lista_wychodzacych.node_ind_end].zapas_czasu == 0)
                    {
                        List<Zdarzenie_t> nowa_sciezka = new List<Zdarzenie_t>(); nowa_sciezka.AddRange(wczesniejsza_sciezka);
                        nowa_sciezka.Add(nodes[lista_wychodzacych.node_ind_end]);

                        path_detected(ref mozliwe_sciezki, nowa_sciezka);
                    }
                }
            }
        }
        private float czas_trwania_pomiedzy_zdarzeniami(int start, int end)
        {
            foreach(var czynnosc in tab_wszystkich_czynnosci)
            {
                if(czynnosc.node_ind_start == start && czynnosc.node_ind_end == end)
                {
                    return czynnosc.czas_trwania;
                }
            }

            return 0;
        }
        int najdluzsza_sciezka(List<List<Zdarzenie_t>> mozliwe_sciezki)
        {
            for(int i=0; i<how_many_nodesss; i++)
            {
                nodes[i].index = i;
            }

            float najdluzsza_dlugosc = -1;
            int najdluzsza_index = -1;

            for (int i=0; i<mozliwe_sciezki.Count; i++)
            {
                var sciezka = mozliwe_sciezki[i];


                float lokalna_dlugosc = 0;
                for (int j = 0; j < sciezka.Count - 1; j++)
                {
                    var node_start = sciezka[j];
                    var node_end = sciezka[j + 1];

                    lokalna_dlugosc += czas_trwania_pomiedzy_zdarzeniami(node_start.index, node_end.index);
                }               

                if(lokalna_dlugosc > najdluzsza_dlugosc)
                {
                    najdluzsza_dlugosc = lokalna_dlugosc;
                    najdluzsza_index = i;
                }
            }

            return najdluzsza_index;
        }
        private List<Zdarzenie_t> wyznaczanie_siezki_krytycznej()
        {
            wyliczanie_zapasu_czasu();
            List<List<Zdarzenie_t>> mozliwe_sciezki = new List<List<Zdarzenie_t>>();

            List<Zdarzenie_t> poczatek = new List<Zdarzenie_t>();
            poczatek.Add(nodes[0]); // dodajemy początkowy

            path_detected(ref mozliwe_sciezki, poczatek);

            int index = najdluzsza_sciezka(mozliwe_sciezki);

            if(index == -1) return null;
            return mozliwe_sciezki[index];
        }
    }
}