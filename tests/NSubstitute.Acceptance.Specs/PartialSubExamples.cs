using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class PartialSubExamples
{
    public class TemplateMethod
    {
        public class FormResult
        {
            public bool IsValid { get; set; }
            public bool IsComplete { get; set; }
        }

        public abstract class Form
        {
            public virtual FormResult Submit()
            {
                var result = Validate();
                if (result.IsValid)
                {
                    Save();
                    result.IsComplete = true;
                }
                return result;
            }

            public abstract FormResult Validate();
            public abstract void Save();
        }

        [Test]
        public void ValidFormSubmission()
        {
            var form = Substitute.ForPartsOf<Form>();
            form.Validate().Returns(new FormResult() { IsValid = true });

            var result = form.Submit();

            Assert.That(result.IsComplete);
            form.Received().Save();
        }

        [Test]
        public void InvalidFormSubmission()
        {
            var form = Substitute.ForPartsOf<Form>();
            form.Validate().Returns(new FormResult() { IsValid = false });

            var result = form.Submit();

            Assert.That(result.IsComplete, Is.False);
            form.DidNotReceive().Save();
        }
    }

    public class TemplateMethod2Example
    {
        public class SummingReader
        {
            public virtual int Read()
            {
                var s = ReadFile();
                return s.Split(',').Select(int.Parse).Sum();
            }
            public virtual string ReadFile() { return "the result of reading the file here"; }
        }

        [Test]
        public void ShouldSumAllNumbersInFile()
        {
            var reader = Substitute.ForPartsOf<SummingReader>();
            reader.ReadFile().Returns("1,2,3,4,5");

            var result = reader.Read();

            Assert.That(result, Is.EqualTo(15));
        }
    }

    public class UnderlyingListExample
    {
        public class TaskList
        {
            readonly List<string> list = [];
            public virtual void Add(string s) { list.Add(s); }
            public virtual string[] ToArray() { return [.. list]; }
        }

        public class TaskView(TaskList tasks)
        {
            public string TaskEntryField { get; set; }
            public string[] DisplayedTasks { get; set; }

            public void ClickButton()
            {
                tasks.Add(TaskEntryField);
                DisplayedTasks = tasks.ToArray();
            }
        }

        [Test]
        public void AddTask()
        {
            var list = Substitute.ForPartsOf<TaskList>();
            var view = new TaskView(list);
            view.TaskEntryField = "write example";

            view.ClickButton();

            // list substitute functions as test spy
            list.Received().Add("write example");
            Assert.That(view.DisplayedTasks, Is.EqualTo(new[] { "write example" }));
        }
    }
}