using System;
using System.Threading;
using System.Windows.Forms;

namespace PaperCuttingMachine
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btn_Print_Click(object sender, EventArgs e)
        {
            tryPring();
        }

        #region 打印相关
        private bool tryPring()
        {
            bool ret = false;
            try
            {
                if(PrinterHelper.TxOpenPrinter(1,0))
                {
                    int status = PrinterHelper.TxGetStatus();
                    if(status==88)
                    {
                        string printText = "";
                        //无故障情况下才执行打印
                        PrinterHelper.TxInit();

                        PrinterHelper.TxResetFont();
                        printText = getPrintTextByStepForBill(1);
                        PrinterHelper.TxOutputStringLn(printText);
                        printText = getPrintTextByStepForBill(2);
                        PrinterHelper.TxOutputStringLn(printText);
                        printText = getPrintTextByStepForBill(3);
                        PrinterHelper.TxOutputStringLn(printText);


                        PrinterHelper.TxDoFunction(10, 240, 0);//走纸30毫米
                        PrinterHelper.TxDoFunction(12, 2, 40);//走纸30毫米

                        Thread.Sleep(1000);
                        bool isSuccess = PrinterHelper.CheckIsPrintSuccess();
                        if (isSuccess)
                        {
                            ret = true;
                            MessageBox.Show("加油单打印成功");
                        }
                        else
                        {
                            MessageBox.Show("打印失败，有可能是打印机内纸不够、打印机断电或其他异常，请确保打印机接上电源并且其内有足够的纸，然后执行一次关闭打印机后再打开打印机。");
                        }
                    }
                    else if (status == 56)
                    {
                        MessageBox.Show("检测到打印机内没有纸，如果有纸，请执行一次关闭打印机后再打开打印机。");
                    }
                    else
                    {
                        MessageBox.Show("打印机繁忙或异常，请尝试执行一次关闭打印机后再打开打印机，可能能解决问题。");
                    }
                }else
                {
                    MessageBox.Show("无法连接打印机，请确保打印机电源打开并且正常连接到电脑");
                }
            }catch(Exception ex)
            {
                ret = false;
                MessageBox.Show("打印时出现异常");
            }finally
            {
                PrinterHelper.TxClosePrinter();
            }
            return ret;
        }


        /// <summary>
        /// 分5个步骤把一个加油单打出来，额外打印加大字号的油品及加油量
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private string getPrintTextByStepForBill(int step)
        {
            string reText = "";
            switch (step)
            {
                case 1:
                    //不带回车、正常打印
                    reText = string.Format(@"加油单
单　　号：{0}
车辆编号：{1}
车主 　 ：{2}
油　　品：{3}　　　　　", "13700700960", "豫N 81996", "土伦", "#95");
                    break;
                case 2:
                    reText = string.Format(@"加油量：{0}", "50L");
                    break;
                case 3:
                    reText = string.Format(@"加油时间：{0}", DateTime.Now.ToString());
                    break;
            }
            return reText;
        }
        #endregion
    }
}
