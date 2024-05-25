using System.Windows;
using UsingTask.Library;
using UsingTask.Shared;

namespace UsingTask.UI;

public partial class MainWindow : Window
{
    PersonReader reader = new();
    CancellationTokenSource? tokenSource;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void FetchWithTaskButton_Click(object sender, RoutedEventArgs e)
    {
        ClearListBox();
        tokenSource = new();

        FetchWithTaskButton.IsEnabled = false;

        Task<List<Person>> peopleTask = reader.GetPeopleAsync(tokenSource.Token);
        peopleTask.ContinueWith(
            task =>
            {
                if (task.IsFaulted)
                {
                    foreach(var ex in task.Exception.Flatten().InnerExceptions)
                    MessageBox.Show($"ERROR\n{ex.GetType()}\n{ex.Message}");
                }
                if (task.IsCanceled)
                {
                    MessageBox.Show("CANCELED CANCELED CANCELED");
                }
                if (task.IsCompletedSuccessfully)
                {
                    List<Person> people = task.Result;
                    foreach (var person in people)
                    {
                        PersonListBox.Items.Add(person);
                    }
                }
                FetchWithTaskButton.IsEnabled = true;
                tokenSource.Dispose();
            },
            TaskScheduler.FromCurrentSynchronizationContext());
    }

    private async void FetchWithAwaitButton_Click(object sender, RoutedEventArgs e)
    {
        ClearListBox();
        tokenSource = new();

        FetchWithAwaitButton.IsEnabled = false;

        try
        {
            List<Person> people = await reader.GetPeopleAsync(tokenSource.Token);
            foreach (var person in people)
            {
                PersonListBox.Items.Add(person);
            }
        }
        catch(OperationCanceledException ex)
        {
            MessageBox.Show($"CANCELED\n{ex.GetType()}\n{ex.Message}");
        }
        catch(Exception ex)
        {
            MessageBox.Show($"ERROR\n{ex.GetType()}\n{ex.Message}");
        }
        finally
        {
            FetchWithAwaitButton.IsEnabled = true;
            tokenSource.Dispose();
        }   
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        tokenSource?.Cancel();
    }

    private void ClearListBox()
    {
        PersonListBox.Items.Clear();
    }
}
