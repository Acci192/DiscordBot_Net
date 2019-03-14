using NUnit.Framework;
using System;
using System.Collections.Generic;
using VoteBot_Discord.Aggregates;
using VoteBot_Discord.Commands;
using VoteBot_Discord.Events;
using VoteBot_Discord.Exceptions;
using VoteBot_Discord.CQRS;

namespace Tests
{
    [TestFixture]
    public class PollTests : BDDTest<PollAggregate>
    {
        private Guid testId;
        private ulong testChannel;
        private string testName;
        private ulong testUser1;
        private ulong testUser2;
        private string testOption1;
        private string testOption2;
        private string testDescription;

        [SetUp]
        public void Setup()
        {
            testId = Guid.NewGuid();
            testChannel = 101;
            testName = "Test";
            testUser1 = 201;
            testUser2 = 202;
            testOption1 = "Option1";
            testOption2 = "Option2";
            testDescription = "Description";
        }

        [Test]
        public void CanCreatePoll()
        {
            Test(
                Given(),
                When(new CreatePoll
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                }),
                Then(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                }));
        }

        [Test]
        public void CanNotAddOptionWithoutCreatedPoll()
        {
            Test(
                Given(),
                When(new AddOptions
                {
                    Id = testId,
                    User = testUser1,
                    Options = new List<string> { testOption1 }
                }),
                ThenFailWith<PollNotCreated>());
        }

        [Test]
        public void UnauthorizedUserCanNotAddOptions()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                }),
                When(new AddOptions
                {
                    Id = testId,
                    User = testUser2,
                    Options = new List<string> { testOption1 }
                }),
                ThenFailWith<UnauthorizedUser>());
        }

        [Test]
        public void CanNotAddOptionsToEndedVote()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                },
                new PollEnded
                {
                    Id = testId
                }),
                When(new AddOptions
                {
                    Id = testId,
                    User = testUser1,
                    Options = new List<string> { testOption1 }
                }),
                ThenFailWith<PollAlreadyEnded>());
        }

        [Test]
        public void CanAddSingleOption()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                }),
                When(new AddOptions
                {
                    Id = testId,
                    User = testUser1,
                    Options = new List<string> { testOption1 }
                }),
                Then(new OptionsAdded
                {
                    Id = testId,
                    Options = new List<string> { testOption1 }
                }));
        }

        [Test]
        public void CanAddMultipleOptions()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                }),
                When(new AddOptions
                {
                    Id = testId,
                    User = testUser1,
                    Options = new List<string> { testOption1, testOption2 }
                }),
                Then(new OptionsAdded
                {
                    Id = testId,
                    Options = new List<string> { testOption1, testOption2 }
                }));
        }

        [Test]
        public void OnlyUniqueOptionsAdded()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                }),
                When(new AddOptions
                {
                    Id = testId,
                    User = testUser1,
                    Options = new List<string> { testOption1, testOption1 }
                }),
                Then(new OptionsAdded
                {
                    Id = testId,
                    Options = new List<string> { testOption1 }
                }));
        }

        [Test]
        public void ExistingOptionsNotAdded()
        {
            Test(Given(new PollCreated
            {
                Id = testId,
                Channel = testChannel,
                Name = testName,
                Owner = testUser1
            }, 
            new OptionsAdded
            {
                Id = testId,
                Options = new List<string> { testOption1 }
            }),
            When(new AddOptions
            {
                Id = testId,
                User = testUser1,
                Options = new List<string> { testOption1, testOption2 }
            }),
            Then(new OptionsAdded
            {
                Id = testId,
                Options = new List<string> { testOption2 }
            }));
        }

        [Test]
        public void CanNotAddEmptyOption()
        {
            Test(Given(new PollCreated
            {
                Id = testId,
                Channel = testChannel,
                Name = testName,
                Owner = testUser1
            },
            new OptionsAdded
            {
                Id = testId,
                Options = new List<string> { testOption1 }
            }),
            When(new AddOptions
            {
                Id = testId,
                User = testUser1,
                Options = new List<string> { testOption1 }
            }),
            ThenFailWith<NoOptionAdded>());
        }

        [Test]
        public void CanNotAddDescriptionWithoutCreatedPoll()
        {
            Test(
                Given(),
                When(new AddDescription
                {
                    Id = testId,
                    Description = testDescription
                }),
                ThenFailWith<PollNotCreated>());
        }

        [Test]
        public void UnauthorizedUserCanNotAddDescription()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                }),
                When(new AddDescription
                {
                    Id = testId,
                    User = testUser2,
                    Description = testDescription
                }),
                ThenFailWith<UnauthorizedUser>());
        }

        [Test]
        public void CanNotAddDescriptionToEndedVote()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                },
                new PollEnded
                {
                    Id = testId
                }),
                When(new AddDescription
                {
                    Id = testId,
                    User = testUser1,
                    Description = testDescription
                }),
                ThenFailWith<PollAlreadyEnded>());
        }

        [Test]
        public void CanAddDescription()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                }),
                When(new AddDescription
                {
                    Id = testId,
                    User = testUser1,
                    Description = testDescription
                }),
                Then(new DescriptionAdded
                {
                    Id = testId,
                    Description = testDescription
                }));
        }

        [Test]
        public void CanNotVoteWithoutCreatedPoll()
        {
            Test(
                Given(),
                When(new PlaceVote
                {
                    Id = testId,
                    User = testUser1,
                    Option = testOption1
                }),
                ThenFailWith<PollNotCreated>());
        }

        [Test]
        public void CanNotVoteOnInvalidOption()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                }),
                When(new PlaceVote
                {
                    Id = testId,
                    User = testUser1,
                    Option = testOption1,
                }),
                ThenFailWith<InvalidOption>());
        }

        [Test]
        public void CanNotVoteOnEndedVote()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                },
                new PollEnded
                {
                    Id = testId
                }),
                When(new PlaceVote
                {
                    Id = testId,
                    User = testUser1,
                    Option = testOption1,
                }),
                ThenFailWith<PollAlreadyEnded>());
        }

        [Test]
        public void CanVoteOnOption()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                },
                new OptionsAdded
                {
                    Id = testId,
                    Options = new List<string> { testOption1 }
                }),
                When(new PlaceVote
                {
                    Id = testId,
                    User = testUser1,
                    Option = testOption1,
                }),
                Then(new VotePlaced
                {
                    Id = testId,
                    User = testUser1,
                    Option = testOption1
                }));
        }

        [Test]
        public void CanNotVoteTwice()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                },
                new OptionsAdded
                {
                    Id = testId,
                    Options = new List<string> { testOption1 }
                },
                new VotePlaced
                {
                    Id = testId,
                    User = testUser1,
                    Option = testOption1
                }),
                When(new PlaceVote
                {
                    Id = testId,
                    User = testUser1,
                    Option = testOption1,
                }),
                ThenFailWith<UserAlreadyVoted>());
        }

        [Test]
        public void UserCanPlaceVoteAfterRemovingVote()
        {
            Test(Given(new PollCreated
            {
                Id = testId,
                Channel = testChannel,
                Name = testName,
                Owner = testUser1
            },
            new OptionsAdded
            {
                Id = testId,
                Options = new List<string> { testOption1 }
            },
            new VotePlaced
            {
                Id = testId,
                User = testUser1,
                Option = testOption1
            },
            new VoteRemoved
            {
                Id = testId,
                User = testUser1
            }),
            When(new PlaceVote
            {
                Id = testId,
                User = testUser1,
                Option = testOption1,
            }),
            Then(new VotePlaced
            {
                Id = testId,
                User = testUser1,
                Option = testOption1
            }));
        }

        [Test]
        public void CanNotRemoveVoteWithoutCreatedVote()
        {
            Test(
                Given(),
                When(new RemoveVote
                {
                    Id = testId,
                    User = testUser1,
                }),
                ThenFailWith<PollNotCreated>());
        }

        [Test]
        public void CanNotRemoveVoteWithoutPlacingVote()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                }),
                When(new RemoveVote
                {
                    Id = testId,
                    User = testUser1,
                }),
                ThenFailWith<UserHasNotVoted>());
        }

        [Test]
        public void CanNotRemoveVoteOnEndedVote()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                },
                new PollEnded
                {
                    Id = testId
                }),
                When(new RemoveVote
                {
                    Id = testId,
                    User = testUser1,
                }),
                ThenFailWith<PollAlreadyEnded>());
        }

        [Test]
        public void CanRemoveVote()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                },
                new OptionsAdded
                {
                    Id = testId,
                    Options = new List<string> { testOption1 }
                },
                new VotePlaced
                {
                    Id = testId,
                    User = testUser1,
                    Option = testOption1
                }),
                When(new RemoveVote
                {
                    Id = testId,
                    User = testUser1,
                }),
                Then(new VoteRemoved
                {
                    Id = testId,
                    User = testUser1
                }));
        }

        [Test]
        public void CanNotEndUncreatedVote()
        {
            Test(
                Given(),
                When(new EndPoll
                {
                    Id = testId
                }),
                ThenFailWith<PollNotCreated>());
        }

        [Test]
        public void CanEndVote()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                }),
                When(new EndPoll
                {
                    Id = testId,
                    User = testUser1
                }),
                Then(new PollEnded
                {
                    Id = testId
                }));
        }

        [Test]
        public void CanNotEndVoteIfUnauthorized()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                }),
                When(new EndPoll
                {
                    Id = testId,
                    User = testUser2
                }),
                ThenFailWith<UnauthorizedUser>());
        }

        [Test]
        public void CanNotEndEndedVote()
        {
            Test(
                Given(new PollCreated
                {
                    Id = testId,
                    Channel = testChannel,
                    Name = testName,
                    Owner = testUser1
                },
                new PollEnded
                {
                    Id = testId
                }),
                When(new EndPoll
                {
                    Id = testId
                }),
                ThenFailWith<PollAlreadyEnded>());
        }
    }
}