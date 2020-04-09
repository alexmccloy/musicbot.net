using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Discord;
using Amccloy.MusicBot.Net.Trivia;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Amccloy.MusicBot.Net.Test.Trivia
{
    [TestClass]
    public class StandardTriviaQuestionsTests
    {
        private const string Question = "This is a test question";
        private const string Answer = "Answer";
        private const string WrongAnswer = "Wrong Answer";

        private const string UserName1 = "Username1";
        private const string UserName2 = "Username2";
        private const string UserName3 = "Username3";
        private const string UserName4 = "Username4";

        /// <summary>
        /// Test that the method can correctly handle only a single answer that is correct
        /// </summary>
        [TestMethod]
        public async Task SingleCorrectAnswer()
        {
            // Arrange
            var (question, scheduler, observable) = CommonArrange(new List<DiscordMessage>()
            {
                new DiscordMessage(null, UserName1, Answer)
            });

            // Act
            var task = question.ExecuteQuestion(observable, TimeSpan.FromSeconds(3));
            scheduler.Start();
            scheduler.Stop();
            var result = await task;
            question.Dispose();

            // Assert
            result.Scores.Count.ShouldBe(1);
            result.Scores[UserName1].ShouldBe(1);
        }

        /// <summary>
        /// Test that the method returns an empty result when no answers are given
        /// </summary>
        [TestMethod]
        public async Task NoAnswers()
        {
            // Arrange
            var (question, scheduler, observable) = CommonArrange(new List<DiscordMessage>());

            // Act
            var task = question.ExecuteQuestion(observable, TimeSpan.FromSeconds(3));
            scheduler.Start();
            scheduler.Stop();
            var result = await task;
            question.Dispose();

            // Assert
            result.Scores.Count.ShouldBe(0);
        }

        private (StandardTriviaQuestion question, TestScheduler scheduler, ITestableObservable<DiscordMessage> observable) 
            CommonArrange(List<DiscordMessage> messages, string question = null, string answer = null)
        {
            var schedulerFactory = new TestSchedulerFactory();
            var triviaQuestion = new StandardTriviaQuestion(question ?? Question, answer ?? Answer, schedulerFactory);
            var scheduler = schedulerFactory.Schedulers[0];

            var observable = ArrangeScheduler(scheduler, messages);

            return (triviaQuestion, scheduler, observable);
        }

        /// <summary>
        /// Adds messages to an observer that will occur every tick
        /// TODO move this to a common class
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="messages"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private ITestableObservable<T> ArrangeScheduler<T>(TestScheduler scheduler, List<T> messages)
        {
            var events = new List<Recorded<Notification<T>>>();
            var time = 0;

            foreach (T message in messages)
            {
                events.Add(new Recorded<Notification<T>>(time++, Notification.CreateOnNext(message)));
            }

            return scheduler.CreateColdObservable(events.ToArray());
        }
    }
}