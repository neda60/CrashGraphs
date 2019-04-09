using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CrashGraphs
{
    public partial class CrashGraph2 : Form
    {
        public CrashGraph2()
        {
            InitializeComponent();
        }
        public static List<String> GetAllFiles(String directory)
        {
            return Directory.GetFiles(directory, "*", SearchOption.AllDirectories).ToList();
        }
        public int Ranks(int[] b, int label, int rank, ref double[] TP, ref double[] FP, ref double[] FN, ref StreamWriter total,/* ref string file,*/ ref int[] lblCount, int NBuckets, int lineNumber)
        {
            /////////////////////////////////////////
            Boolean found = false;
            int rnk = -1;// rank position of the bucket label

            //List<double> rnk = new List<double>();
            for (int r = 1; r <= rank; r++)
            {
                if (b[b.Length - 1 - (rank - 1)] == label) // because b is ordered ascending and we need descending order
                {
                    found = true;
                    rnk = r;
                    break;
                }
            }

            if (found)
            {
                TP[label - 1]++;
            }
            else
            {
                FN[label - 1]++;
                FP[b[0] - 1]++;  //nafahmidam
            }
            //lblIndex++;


            return rnk + 1;


        }


        private void button1_Click(object sender, EventArgs e)
        {
            //Creating Crash Graphs for training data
            //for (int B = 1; B < 10; B++)
            foreach (string B in GetAllFiles(txtTrain.Text))
            //foreach (string file2 in DirTrains)
            {
                //StreamReader f1 = new StreamReader(@"C:\project\Test Data\2 Unique traces\" + B + ".txt");
                StreamReader f1 = new StreamReader(B);
                Dictionary<string, int> dictionary2 = new Dictionary<string, int>();// making the hash table

                string line;
                string[] str;
                //foreach (string file in DirFiles)
                while (!f1.EndOfStream)
                {
                    line = f1.ReadLine();
                    str = line.Trim().Split(' ');

                    if (str.Length == 1)
                    {
                        if (!dictionary2.ContainsKey(str[0]))
                            dictionary2.Add(str[0], 1);
                    }
                    else
                    {
                        for (int i = 0; i < str.Length - 1; i++)
                        {
                            if (!dictionary2.ContainsKey(str[i] + " " + str[i + 1]))
                                dictionary2.Add(str[i] + " " + str[i + 1], 1);
                            else
                                dictionary2[str[i] + " " + str[i + 1]]++;

                        }
                    }
                } // end while
                // writing crash graph in files e.g. Crash_1 => Crash Graph for bucket 1
                //StreamWriter writer = new StreamWriter(@"C:\project\Test Data\10 Crash Graph\Graph_" + B);
                StreamWriter writer = new StreamWriter(txtDest.Text + "\\" + B.Split('\\')[B.Split('\\').Length - 1]);
                foreach (KeyValuePair<string, int> pair in dictionary2)
                    writer.WriteLine(pair.Key);

                writer.Close();
            }// end B
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StreamReader res = new StreamReader(txtDest.Text + "\\Result\\Results.txt");
            string[] str;
            Dictionary<string, int> dicST = new Dictionary<string, int>();// making the hash table
            Dictionary<string, int> dicTP = new Dictionary<string, int>();// making the hash table
            Dictionary<string, int> dicFP = new Dictionary<string, int>();// making the hash table
            Dictionary<string, int> dicFN = new Dictionary<string, int>();// making the hash table

            while (!res.EndOfStream)
            {
                //calculating TP rate
                str = res.ReadLine().Split('\t');
                if (!dicST.ContainsKey(str[0]))
                {
                    dicST.Add(str[0], 1);
                    if (!dicTP.ContainsKey(str[0])) dicTP.Add(str[0], 0);
                    if (!dicFP.ContainsKey(str[0])) dicFP.Add(str[0], 0);
                    if (!dicFN.ContainsKey(str[0])) dicFN.Add(str[0], 0);
                }
                else dicST[str[0]]++;

                if (str[1] == str[0])
                {
                    dicTP[str[0]]++;
                }
                else //if (!dicFN.ContainsKey(str[0]))// calculating FP and FN
                {
                    dicFN[str[0]]++;
                    if (!dicFP.ContainsKey(str[1]))
                    {
                        dicFP.Add(str[1], 1);
                    }
                    else dicFP[str[1]]++;

                }
            }

            StreamWriter confWriter = new StreamWriter(txtDest.Text + "\\Result\\TPFP.txt");
            confWriter.WriteLine("bucket_ID\tTP\tFP\tFN\tTPR\tFPR\tFNR");
            foreach (var pair in dicST)
            {
                confWriter.WriteLine(pair.Key + "\t" + dicTP[pair.Key] + "\t" + dicFP[pair.Key] + "\t" + dicFN[pair.Key] + "\t" + Convert.ToDouble(dicTP[pair.Key]) / pair.Value + "\t" + Convert.ToDouble(dicFP[pair.Key]) / (dicST.Count - pair.Value) + "\t" + Convert.ToDouble(dicFN[pair.Key]) / pair.Value);
            }

            confWriter.Close();

            //int[] matrix = new int[10];
            //string line;
            //string[] str;
            ////StreamWriter writer = new StreamWriter(@"C:\project\Test Data\11 result table\CrashGraph.txt", true);
            //StreamWriter writer = new StreamWriter(@"C:\project\Test Data\11 result table\CrashGraph.txt", true);


            //for (int B = 1; B < 10; B++)
            //{


            //    for (int i = 0; i < 10; i++) { matrix[i] = 0; }
            //    StreamReader reader = new StreamReader(@"C:\project\Test Data\10 Crash Graph\" + B + ".txt");

            //    while (!reader.EndOfStream)
            //    {
            //        line = reader.ReadLine();
            //        str = line.Split('|');
            //        if (str[0] == " ")
            //        {
            //            break;
            //        }
            //        else if (str[1] != "")
            //        {
            //            matrix[Convert.ToInt16(str[1]) - 1]++;
            //        }
            //        else if (str[2] != "")
            //            matrix[Convert.ToInt16(str[2].Split('_')[0]) - 1]++;
            //        else if (str[3] == "R")
            //            matrix[9]++;

            //    }
            //    //writer.Write(B + "|");
            //    for (int i = 0; i < 9; i++)
            //    {
            //        writer.Write(matrix[i] + "\t");
            //    }
            //    writer.WriteLine(matrix[9]);
            //}

            //writer.Close();
        }

        private void btnCrashGraph_Click(object sender, EventArgs e)
        {
            string line, item;
            string[] str;
            Dictionary<string, int> dic1 = new Dictionary<string, int>();

            StreamWriter myW = new StreamWriter(txtDest.Text + "\\Result\\Results.txt");

            foreach (string testTraces in GetAllFiles(txtTest.Text))
            {
                StreamReader testTrace = new StreamReader(testTraces);
                int traceB = int.Parse(testTraces.Split('\\')[testTraces.Split('\\').Length - 1].Split('.')[0]);

                while (!testTrace.EndOfStream)
                {
                    line = testTrace.ReadLine().Trim();
                    str = line.Split(' ');
                    if (str.Length == 1)
                    {
                        if (!dic1.ContainsKey(str[0]))
                            dic1.Add(str[0], 1);
                    }
                    else
                    {
                        for (int count = 0; count < str.Length - 1; count++)
                        {
                            //if (str[count] != "")
                            //{
                            item = str[count] + " " + str[count + 1];
                            if (!dic1.ContainsKey(item)) //str[count] + " " + str[count + 1] I use dictionary to facilitate the process of searching for next frequency in sequences
                            {// in order to prevent adding the non containing sequences to the dictionary
                                dic1.Add(item, 1);
                            }

                        }
                    }


                    double temp = 0, rejected_sim = 0;
                    int relatedBucket = 0, partly_similar = 0, rejected = 0;

                    // read each line (stack trace) and compre with other buckets
                    foreach (string B in GetAllFiles(txtTrain.Text))
                    {
                        StreamReader f1 = new StreamReader(txtDest.Text + "\\" + B.Split('\\')[B.Split('\\').Length - 1]);// reading Graph files to keep them in hash table for furthere comparison with test traces

                        Dictionary<string, int> dic2 = new Dictionary<string, int>();// making the hash table

                        //foreach (string file in DirFiles)
                        while (!f1.EndOfStream)
                        {
                            line = f1.ReadLine().Trim();
                            if (!dic2.ContainsKey(line)) // adding 2-grams to the training dictionary
                                dic2.Add(line, 1);

                            str = line.Split(' ');

                            for (int i = 0; i < str.Length; i++) // adding 1-grams to the training dictionary
                            {
                                if (!dic2.ContainsKey(str[i]))
                                    dic2.Add(str[i], 1);
                                //else             // I do not need the sequences
                                //   dictionary2[str[i] + " " + str[i + 1]]++;

                            }
                        }
                        int countSimilar = 0;

                        foreach (KeyValuePair<string, int> pair in dic1)
                            if (dic2.ContainsKey(pair.Key))
                            {
                                countSimilar++;
                            }

                        double similarity = 0;
                        similarity = Convert.ToDouble(countSimilar) / Math.Min(dic1.Count, dic2.Count);
                        countSimilar = 0;

                        int bucket = int.Parse(B.Split('\\')[B.Split('\\').Length - 1].Split('.')[0]);
                        if (similarity > temp) // >=
                        {/*
                            if(similarity==temp && bucket == traceB && similarity>0)
                            {
                                relatedBucket = bucket;
                            }
                            else  if (similarity>temp)
                            {*/
                            temp = similarity;
                            relatedBucket = bucket;
                            // }

                        }
                        //if (similarity == 1 && relatedBucket == 0)
                        //{
                        //    relatedBucket = bucket;
                        //    temp = similarity;
                        //}
                        //else if (similarity >= 0.95 && similarity > temp)
                        //{
                        //    partly_similar = bucket;
                        //    temp = similarity;
                        //}
                        //else if (similarity > temp)
                        //    temp = similarity;
                        //    rejected = bucket;

                        dic2.Clear();
                    }

                    myW.WriteLine(traceB + "\t" + relatedBucket + "\t" + temp);
                    //if (relatedBucket != 0)
                    //    {    // trace name | Related Bucket | Similar Bucket | Rejected
                    //        myW.WriteLine(traceB + "|" + relatedBucket + "|" + "|");
                    //        //totalSame++;
                    //    }
                    //    else
                    //    {
                    //        if (partly_similar != 0)
                    //        {
                    //            myW.WriteLine(traceB + "|" + "|" + partly_similar + "_" + temp.ToString() + "|");
                    //            //totalSimilar++;
                    //        }
                    //        else
                    //        {
                    //            myW.WriteLine(traceB + "|" + "|" + "|" + rejected + "_" + temp.ToString());
                    //            //totalR++;
                    //        }
                    //    }

                    dic1.Clear();
                    //myW.WriteLine(" " + "|" + totalSame + "|" + totalSimilar + "|" + totalR);
                    //totalSame = 0; totalSimilar = 0; totalR = 0;


                    /////////////////////////////////////////

                    //myW.WriteLine(" " + "|" + totalSame + "|" + totalSimilar + "|" + totalR);
                    dic1.Clear();
                }
                testTrace.Close();

            }
            myW.Close();


        }

        private void txtTrain_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string line, item;
            string[] str;
            Dictionary<string, int> dic1 = new Dictionary<string, int>();

            StreamWriter myW = new StreamWriter(txtDest.Text + "\\Result\\Results.txt");

            foreach (string testTraces in GetAllFiles(txtTest.Text))
            {
                StreamReader testTrace = new StreamReader(testTraces);
                int traceB = int.Parse(testTraces.Split('\\')[testTraces.Split('\\').Length - 1].Split('.')[0]);

                while (!testTrace.EndOfStream)
                {
                    line = testTrace.ReadLine().Trim();
                    str = line.Split(' ');
                    if (str.Length == 1)
                    {
                        if (!dic1.ContainsKey(str[0]))
                            dic1.Add(str[0], 1);
                    }
                    else
                    {
                        for (int count = 0; count < str.Length - 1; count++)
                        {
                            //if (str[count] != "")
                            //{
                            item = str[count] + " " + str[count + 1];
                            if (!dic1.ContainsKey(item)) //str[count] + " " + str[count + 1] I use dictionary to facilitate the process of searching for next frequency in sequences
                            {// in order to prevent adding the non containing sequences to the dictionary
                                dic1.Add(item, 1);
                            }

                        }
                    }


                    double temp = 0;
                    int relatedBucket = 0;

                    // read each line (stack trace) and compre with other buckets
                    foreach (string B in GetAllFiles(txtTrain.Text))
                    {
                        StreamReader f1 = new StreamReader(txtDest.Text + "\\" + B.Split('\\')[B.Split('\\').Length - 1]);// reading Graph files to keep them in hash table for furthere comparison with test traces

                        Dictionary<string, int> dic2 = new Dictionary<string, int>();// making the hash table

                        //foreach (string file in DirFiles)
                        while (!f1.EndOfStream)
                        {
                            line = f1.ReadLine().Trim();
                            if (!dic2.ContainsKey(line)) // adding 2-grams to the training dictionary
                                dic2.Add(line, 1);

                            str = line.Split(' ');

                            for (int i = 0; i < str.Length; i++) // adding 1-grams to the training dictionary
                            {
                                if (!dic2.ContainsKey(str[i]))
                                    dic2.Add(str[i], 1);
                                //else             // I do not need the sequences
                                //   dictionary2[str[i] + " " + str[i + 1]]++;

                            }
                        }
                        int countSimilar = 0;

                        foreach (KeyValuePair<string, int> pair in dic1)
                            if (dic2.ContainsKey(pair.Key))
                            {
                                countSimilar++;
                            }

                        double similarity = 0;
                        similarity = Convert.ToDouble(countSimilar) / Math.Min(dic1.Count, dic2.Count);
                        countSimilar = 0;

                        int bucket = int.Parse(B.Split('\\')[B.Split('\\').Length - 1].Split('.')[0]);
                        if (similarity > temp) // >=
                        {

                            temp = similarity;
                            relatedBucket = bucket;
                            // }

                        }


                        dic2.Clear();
                    }

                    myW.WriteLine(traceB + "\t" + relatedBucket + "\t" + temp);


                    dic1.Clear();
                    //myW.WriteLine(" " + "|" + totalSame + "|" + totalSimilar + "|" + totalR);
                    //totalSame = 0; totalSimilar = 0; totalR = 0;


                    /////////////////////////////////////////

                    //myW.WriteLine(" " + "|" + totalSame + "|" + totalSimilar + "|" + totalR);
                    dic1.Clear();
                }
                testTrace.Close();

            }
            myW.Close();

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            string line, item;
            string[] str;
            Dictionary<string, int> dic1 = new Dictionary<string, int>();

            StreamWriter myW = new StreamWriter(txtDest.Text + "\\Result\\Scores.txt");

            foreach (string testTraces in GetAllFiles(txtTest.Text))
            {
                StreamReader testTrace = new StreamReader(testTraces);
                int traceB = int.Parse(testTraces.Split('\\')[testTraces.Split('\\').Length - 1].Split('.')[0]);

                while (!testTrace.EndOfStream)
                {
                    line = testTrace.ReadLine().Trim();
                    str = line.Split(' ');
                    if (str.Length == 1)
                    {
                        if (!dic1.ContainsKey(str[0]))
                            dic1.Add(str[0], 1);
                    }
                    else
                    {
                        for (int count = 0; count < str.Length - 1; count++)
                        {
                            //if (str[count] != "")
                            //{
                            item = str[count] + " " + str[count + 1];
                            if (!dic1.ContainsKey(item)) //str[count] + " " + str[count + 1] I use dictionary to facilitate the process of searching for next frequency in sequences
                            {// in order to prevent adding the non containing sequences to the dictionary
                                dic1.Add(item, 1);
                            }

                        }
                    }




                    // read each line (stack trace) and compre with other buckets
                    foreach (string B in GetAllFiles(txtTrain.Text))
                    {
                        StreamReader f1 = new StreamReader(txtDest.Text + "\\" + B.Split('\\')[B.Split('\\').Length - 1]);// reading Graph files to keep them in hash table for furthere comparison with test traces

                        Dictionary<string, int> dic2 = new Dictionary<string, int>();// making the hash table

                        //foreach (string file in DirFiles)
                        while (!f1.EndOfStream)
                        {
                            line = f1.ReadLine().Trim();
                            if (!dic2.ContainsKey(line)) // adding 2-grams to the training dictionary
                                dic2.Add(line, 1);

                            str = line.Split(' ');

                            for (int i = 0; i < str.Length; i++) // adding 1-grams to the training dictionary
                            {
                                if (!dic2.ContainsKey(str[i]))
                                    dic2.Add(str[i], 1);

                            }
                        }
                        int countSimilar = 0;

                        foreach (KeyValuePair<string, int> pair in dic1)
                            if (dic2.ContainsKey(pair.Key))
                            {
                                countSimilar++;
                            }

                        double similarity = 0;
                        similarity = Convert.ToDouble(countSimilar) / Math.Min(dic1.Count, dic2.Count);
                        myW.Write(similarity + " ");

                        //countSimilar = 0;

                        //int bucket = int.Parse(B.Split('\\')[B.Split('\\').Length - 1].Split('.')[0]);
                        //if (similarity > temp) // >=
                        //{
                        //    temp = similarity;
                        //    relatedBucket = bucket;
                        //    // }

                        //}


                        dic2.Clear();
                    }

                    //myW.WriteLine(traceB + "\t" + relatedBucket + "\t" + temp);
                    myW.WriteLine();

                    dic1.Clear();
                }
                testTrace.Close();

            }
            myW.Close();

        }

        private void button4_Click(object sender, EventArgs e)
        {

            int NBuckets = Convert.ToInt16(txtNBuckets.Text);
            int NTraces = Convert.ToInt16(txtNTraces.Text);
            //List<double> percent = new List<double>();
            //double d;
            //int lblIndex = 0; // lblIndex to keep index of min score between all buckets
            int lineNumber = 0; // keepng the line number in labels
            //int[] scores = new int[Convert.ToInt16(txtNTraces.Text)];// keeps the result of related bucket to be compared with real labels
            //List<List<double>> scores = new List<List<double>>();// 2D list to keep scores of all buckets
            //List<String> scoreFiles = GetScoreFiles(txtScores.Text);
            //double[] TP = new double[NBuckets]; double[] FN = new double[NBuckets];
            //double[] FP = new double[NBuckets]; double[] precision = new double[NBuckets]; //, FN = new double[NBuckets] , precision = new double[NBuckets]; 


            StreamReader lbl = new StreamReader(txtResult.Text + "\\labels.txt");

            string[] label = new string[Convert.ToInt16(txtNTraces.Text)];// to the lable of each trace
            Dictionary<string, int> lblCount = new Dictionary<string, int>(); // to keep the number of traces in each bucket
            string ss;
            int l = 0;
            //Dictionary<int, double> dicTP = new Dictionary<int, double>();
            //Dictionary<int, double> dicFP = new Dictionary<int, double>();
            //Dictionary<string, int> dicST = new Dictionary<string, int>();// making the hash table
            //Dictionary<string, int> dicTP = new Dictionary<string, int>();// making the hash table
            //Dictionary<string, int> dicFP = new Dictionary<string, int>();// making the hash table
            //Dictionary<string, int> dicFN = new Dictionary<string, int>();// making the hash table




            while (!lbl.EndOfStream)
            {
                ss = lbl.ReadLine().Trim();
                label[l] = ss;
                if (label[l] != "")
                {
                    if (!lblCount.ContainsKey(ss))
                    {
                        lblCount.Add(ss, 1); // index of Bucket# is -1 less
                        //dicTP.Add(ss, 0);
                        //dicFP.Add(ss, 0);
                    }
                    else
                        lblCount[ss]++;
                }
                l++;
            }
            lbl.Close();
            //List<int> rank_position = new List<int>(); //keep the rank position of the right rank
            Dictionary<int, int> rank_position = new Dictionary<int, int>();
            for (int rank = 1; rank <= 20; rank++)
            {
                Dictionary<string, int> dicTP = new Dictionary<string, int>();// making the hash table
                Dictionary<string, int> dicFP = new Dictionary<string, int>();// making the hash table
                foreach (var v in lblCount)
                {
                    dicTP.Add(v.Key, 0);
                    dicFP.Add(v.Key, 0);
                }

                StreamWriter total = new StreamWriter(txtResult.Text + "\\Rank" + rank + "_Accuracy_total.txt");
                total.WriteLine("B#\tTot\tTP\tFP\tTPR\tFPR\tPre\tRec\tAcc");


                StreamReader f1 = new StreamReader(txtResult.Text + "\\Scores.txt");
                lineNumber = 0; // 
                string[] str;
                while (!f1.EndOfStream)
                {
                    List<double> sc = new List<double>();
                    //double[] scr = new double[NBuckets];
                    //str = f1.ReadLine().Split('\t');
                    str = f1.ReadLine().Split(' ');
                    foreach (string s in str)
                    {
                        if (s != "")
                        {
                            sc.Add(Convert.ToDouble(s));
                        }
                    }
                    double[] scr = sc.ToArray(); //new double[sc.Count];
                    int[] b = new int[NBuckets];// an array to keet the sequence of bucket numbers of scores
                    int i = 0;
                    foreach (string B in GetAllFiles(txtTest.Text))
                    {
                        b[i] = int.Parse(B.Split('\\')[B.Split('\\').Length - 1].Split('.')[0]);
                        i++;
                    }
                    Array.Sort(scr, b); // sorts scr while keeping b in their relevant position
                    //
                    int index = Array.IndexOf(b, int.Parse(label[lineNumber]));
                    if (!rank_position.ContainsKey(lineNumber))
                        rank_position.Add(lineNumber, index);

                    Boolean found = false;
                    //int rnk = -1;// rank position of the bucket label

                    //List<double> rnk = new List<double>();
                    for (int r = 1; r <= rank; r++)
                    {
                        if ((b[b.Length - 1 - (r - 1)]) == int.Parse(label[lineNumber])) // because b is ordered ascending and we need descending order
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {

                        dicTP[label[lineNumber]]++;
                    }
                    else
                    {

                        dicFP[b[b.Length - 1].ToString()]++;
                    }

                    lineNumber++;

                }
                lineNumber--;
                foreach (var pairs in lblCount)
                {
                    total.WriteLine(pairs.Key + "\t" + pairs.Value + "\t" + dicTP[pairs.Key] + "\t" + dicFP[pairs.Key] + "\t" + Convert.ToDouble(dicTP[pairs.Key]) / pairs.Value);
                }
                total.Close();
                //foreach (string p in dicTP.Keys.ToList())
                //{
                //    dicTP[p] = 0;
                //    dicFP[p] = 0;
                //}

            }
            //scoreWriter.Close();
            float Q = 0;
            StreamWriter mapWriter = new StreamWriter(txtResult.Text + "\\MAP.txt");
            foreach (var pair in rank_position)
            {
                //rankWriter.WriteLine(n);
                mapWriter.WriteLine(pair.Key + "\t" + (lblCount.Count - pair.Value));
                Q = Q + (((float)1) / (float)(lblCount.Count - pair.Value)); // because the dictionary is soreted ascending
            }
            Q = (Q / rank_position.Count);
            mapWriter.WriteLine("MAP is: " + Q);
            mapWriter.Close();
            ////Q = Q * (1 / (float)(int.Parse(txtNTraces.Text)));//rank_position.Count
            ////rankWriter.Close();
            ////map.WriteLine(Q);
            ////rank_position.Clear();

            ////StreamWriter accuracy = new StreamWriter(dir + "\\Accuracy" + file.Substring(file.Length - 6, 2) + ".txt");
            ////accuracy.WriteLine("B#\tTot\tTP\tFP\tTPR\tFPR\tFN\tPre\tRec");
            ////for (int x = 0; x < NBuckets; x++)
            ////{
            ////    accuracy.WriteLine((x + 1) + "\t" + lblCount[x] + "\t" + string.Format("{0:0.00}", (TP[x])) + "\t" + string.Format("{0:0.00}", (FP[x])) + "\t" + string.Format("{0:0.00}", (TP[x] / (TP[x] + FN[x]))) + "\t" + string.Format("{0:0.00}", (FP[x] / (int.Parse(txtNTraces.Text) - lblCount[x]))) + "\t" + FN[x] + "\t" + string.Format("{0:0.00}", (TP[x] / (TP[x] + FP[x]))) + "\t" + string.Format("{0:0.00}", (TP[x] / (TP[x] + FN[x]))));
            ////    total.WriteLine(file.Substring(file.Length - 6, 2) + "\t" + (x + 1) + "\t" + lblCount[x] + "\t" + TP[x] + "\t" + FP[x] + "\t" + string.Format("{0:0.00}", (TP[x] / (TP[x] + FN[x]))) + "\t" + string.Format("{0:0.00}", (FP[x] / (int.Parse(txtNTraces.Text) - lblCount[x]))) + "\t" + FN[x] + "\t" + string.Format("{0:0.00}", (TP[x] / (TP[x] + FP[x]))) + "\t" +
            ////    string.Format("{0:0.00}", (TP[x] / (TP[x] + FN[x]))) + "\t" + string.Format("{0:0.00}", (TP[x] / lblCount[x])));

            ////}
            //////accuracy.WriteLine(" \t" + lblCount.Sum() + "\t" + TP.Sum() + "\t" + FP.Sum() + "\t" + FN.Sum() + "\t" + "{0:0.00}" + "\t" + "{0:0.00}", (TP.Sum() / (TP.Sum() + FP.Sum())), (TP.Sum() / (TP.Sum() + FN.Sum())));
            ////accuracy.Close();

            //////}

            ////total.Close();
            ////map.Close();


        }

        private void button5_Click(object sender, EventArgs e)
        {
            StreamWriter w = new StreamWriter(txtDest.Text + "\\Result\\labels.txt");
            foreach (string B in GetAllFiles(txtTest.Text))
            {
                StreamReader f = new StreamReader(B);
                int line = (f.ReadToEnd().Split('\n').Length - 1);
                for (int i = 0; i < line; i++)
                {
                    w.WriteLine(B.Split('\\')[B.Split('\\').Length - 1].Split('.')[0]);
                }
                f.Close();
            }
            w.Close();
        }



        ////// Comparing test cases against training

        //////for (int B = 1; B < 10 ; B++)
        ////// foreach( string B in GetAllFiles(""))
        ////// {

        //////List<String> DirTracess = GetAllFiles(@"C:\test\auto_seq\test data\TESTING DATA\Test\Automata IDs\" + txtTestAlpha.Text + "\\" + txtTestBucket.Text + "\\");//  + "\\" + file.Split('\\')[file.Split('\\').Length - 1].Split('_')[1].Substring(0, file.Split('\\')[file.Split('\\').Length - 1].Split('_')[1].Length - 4)      C:\test\auto_seq\test data\TESTING DATA\Training\automata\ getting all files in directory
        //////List<String> DirTracess = GetAllFiles(@"C:\test\auto_seq\test data\TESTING DATA\Test\Automata IDs\" + buckets[j] + "\\");//  + "\\" + file.Split('\\')[file.Split('\\').Length - 1].Split('_')[1].Substring(0, file.Split('\\')[file.Split('\\').Length - 1].Split('_')[1].Length - 4)      C:\test\auto_seq\test data\TESTING DATA\Training\automata\ getting all files in directory
        ////List<String> DirTests = GetAllFiles(txtTest.Text);
        ////string line, item;
        ////int relatedBucket, partly_similar;
        ////string[] str;
        ////Dictionary<string, int> dictionary = new Dictionary<string, int>();// making the hash table

        ////int totalSame = 0, totalSimilar = 0, totalR = 0;

        ////foreach (string testTraces in DirTests)
        ////{
        ////    StreamWriter myW = new StreamWriter(txtDest.Text + "\\Result\\" + testTraces.Split('\\')[testTraces.Split('\\').Length - 1], true);

        ////    relatedBucket = 0; partly_similar = 0;
        ////    StreamReader testTrace = new StreamReader(testTraces);// Reading test trace IDs

        ////    while (!testTrace.EndOfStream) // Finding unique sequences in traces to make a Crash Graph
        ////    {
        ////        line = testTrace.ReadLine().Trim();
        ////        str = line.Split(' ');
        ////        if (str.Length == 1)
        ////        {
        ////            if (!dictionary.ContainsKey(str[0]))
        ////                dictionary.Add(str[0], 1);
        ////        }
        ////        else
        ////        {
        ////            for (int count = 0; count < str.Length - 1; count++)
        ////            {
        ////                //if (str[count] != "")
        ////                //{
        ////                item = str[count] + " " + str[count + 1];
        ////                if (!dictionary.ContainsKey(item)) //str[count] + " " + str[count + 1] I use dictionary to facilitate the process of searching for next frequency in sequences
        ////                {// in order to prevent adding the non containing sequences to the dictionary
        ////                    dictionary.Add(item, 1);
        ////                }

        ////                //}
        ////            }
        ////        }

        ////    }
        ////    testTrace.Close();
        ////    ////////////////////////////////////////////////////


        ////    //List<String> DirTrains = GetAllFiles(@"C:\project\Test Data\2 Unique traces\");// getting all files in directory

        ////    line = "";
        ////    int countSimilar = 0;
        ////    double temp = 0;

        ////    //List<String> myStr = new List<string>();
        ////    foreach (string B in GetAllFiles(txtTrain.Text))
        ////     //for (int B2 = 1; B2 < 10; B2++)
        ////    {
        ////        //StreamReader f1 = new StreamReader(@"C:\project\Test Data\10 Crash Graph\Graph_" + B2);// reading Graph files to keep them in hash table for furthere comparison with test traces
        ////        StreamReader f1 = new StreamReader(txtDest.Text + "\\" + B.Split('\\')[B.Split('\\').Length - 1]);// reading Graph files to keep them in hash table for furthere comparison with test traces

        ////        Dictionary<string, int> dictionary2 = new Dictionary<string, int>();// making the hash table
        ////        countSimilar = 0;

        ////        //foreach (string file in DirFiles)
        ////        while (!f1.EndOfStream)
        ////        {
        ////            line = f1.ReadLine().Trim();
        ////            if (!dictionary2.ContainsKey(line)) // adding 2-grams to the training dictionary
        ////                dictionary2.Add(line, 1);

        ////            str = line.Split(' ');

        ////            for (int i = 0; i < str.Length; i++) // adding 1-grams to the training dictionary
        ////            {
        ////                if (!dictionary2.ContainsKey(str[i]))
        ////                    dictionary2.Add(str[i], 1);
        ////                //else             // I do not need the sequences
        ////                //   dictionary2[str[i] + " " + str[i + 1]]++;

        ////            }

        ////            foreach (KeyValuePair<string, int> pair in dictionary)
        ////                if (dictionary2.ContainsKey(pair.Key))
        ////                {
        ////                    countSimilar++;
        ////                }

        ////            double similarity = 0;
        ////            similarity = Convert.ToDouble(countSimilar) / Math.Min(dictionary.Count, dictionary2.Count);
        ////            countSimilar = 0;

        ////        int bucket = int.Parse(B.Split('\\')[B.Split('\\').Length - 1].Split('.')[0]);
        ////        if (similarity == 1 && relatedBucket == 0)
        ////        {
        ////            relatedBucket = bucket;
        ////        }
        ////        else if (similarity >= 0.95 && similarity > temp)
        ////        {
        ////            partly_similar = bucket;
        ////            temp = similarity;
        ////        }

        ////        dictionary2.Clear();
        ////        }



        ////        //dictionary2.Clear();

        ////        //StreamWriter f4 = new StreamWriter(path + "\\" + txtDirectory.Text + "_similarity.txt", true);



        ////    } // end of reading traing sets


        ////    if (relatedBucket != 0)
        ////    {    // trace name | Related Bucket | Similar Bucket | Rejected
        ////        myW.WriteLine(testTraces.Split('\\')[testTraces.Split('\\').Length - 1] + "|" + relatedBucket + "|" + "|");
        ////        totalSame++;
        ////    }
        ////    else
        ////    {
        ////        if (partly_similar != 0)
        ////        {
        ////            myW.WriteLine(testTraces.Split('\\')[testTraces.Split('\\').Length - 1] + "|" + "|" + partly_similar + "_" + temp.ToString() + "|");
        ////            totalSimilar++;
        ////        }
        ////        else
        ////        {
        ////            myW.WriteLine(testTraces.Split('\\')[testTraces.Split('\\').Length - 1] + "|" + "|" + "|" + "R");
        ////            totalR++;
        ////        }
        ////    }

        ////    dictionary.Clear();
        ////    myW.WriteLine(" " + "|" + totalSame + "|" + totalSimilar + "|" + totalR);
        ////    myW.Close();
        ////    totalSame = 0; totalSimilar = 0; totalR = 0;
        ////}

        ///////////////////////////////////////////////

        //////myW.WriteLine(" " + "|" + totalSame + "|" + totalSimilar + "|" + totalR);
        //////myW.Close();
        //////}// ENd of for j for buckets

    }


}

