using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotaParana2
{
    public static class ThreadHelperClass
    {
        delegate void SetTextCallback(Form f, Control ctrl, string text);
        delegate void AddMaximumProgressCallback(Form form, ProgressBar prog, int value);
        delegate void StepProgressCallback(Form form, ProgressBar prog);
        /// <summary>
        /// Set text property of various controls
        /// </summary>
        /// <param name="form">The calling form</param>
        /// <param name="ctrl"></param>
        /// <param name="text"></param>
        public static void SetText(Form form, Control ctrl, string text)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (ctrl.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                form.Invoke(d, new object[] { form, ctrl, text });
            }
            else
            {
                ctrl.Text = text;
            }
        }
        public static void AddMaximumProgress(Form form, ProgressBar prog, int value)
        {
            if (prog.InvokeRequired)
            {
                AddMaximumProgressCallback d = new AddMaximumProgressCallback(AddMaximumProgress);
                form.Invoke(d, new object[] { form, prog, value });
            }
            else
            {
                prog.Maximum = value;
            }
        }
        public static void StepProgress(Form form, ProgressBar prog)
        {
            if (prog.InvokeRequired)
            {
                StepProgressCallback d = new StepProgressCallback(StepProgress);
                form.Invoke(d, new object[] { form, prog });
            }
            else
            {
                prog.PerformStep();
            }
        }
    }
}
