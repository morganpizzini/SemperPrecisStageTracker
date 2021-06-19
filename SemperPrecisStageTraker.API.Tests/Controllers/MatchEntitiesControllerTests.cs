using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemperPrecisStageTracker.API.Controllers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTraker.API.Tests.Controllers.Common;

namespace SemperPrecisStageTraker.API.Tests.Controllers
{
    [TestClass]
    public partial class MatchEntitiesControllerTests : ApiControllerTestsBase<MatchController, SimpleScenario>
    {
        [TestMethod]
        public async Task ShouldFetchAvailableShooterSOStagesBeOkHavingProvidedData()
        {
            // get stage with any shooterStage
            var existingStage = Scenario.Stages.FirstOrDefault(x=> Scenario.ShooterSOStages.Any(s=>s.StageId != x.Id));
            
            if(existingStage== null)
                Assert.Inconclusive("Stage not found");

            var existingMatch = Scenario.Matches.FirstOrDefault(x=> x.Id== existingStage.MatchId);
            if(existingMatch== null)
                Assert.Inconclusive("Match not found");
            
            var shooterAssociations = Scenario.ShooterAssociations.Where(x => x.SafetyOfficier && x.AssociationId == existingMatch.AssociationId)
                .Select(x=>x.ShooterId).ToList();

            var stagesInMatch = Scenario.Stages.Where(x => x.MatchId == existingMatch.Id).Select(x => x.Id).ToList();
            
            var existingShooterSo = Scenario.ShooterSOStages.Where(x => stagesInMatch.Contains(x.StageId)).Select(x=>x.ShooterId);

            // not a match director
            var shooterMatches= Scenario.ShooterMatches.Where(x => x.MatchId == existingMatch.Id).Select(x => x.ShooterId).ToList();
            
            var shooterIds = Scenario.Shooters.Where(x =>
                shooterAssociations.Contains(x.Id) && !shooterMatches.Contains(x.Id) && !existingShooterSo.Contains(x.Id)).Select(x=>x.Id).ToList();

            if(shooterIds.Count<0)
                Assert.Inconclusive("Shooters not found");
            
            //Composizione della request
            var request = new StageRequest()
            {
                StageId = existingStage.Id
            };

            //Invoke del metodo
            var response = await Controller.FetchAvailableStageSO(request);
            
            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<ShooterContract>>(response);

            Assert.IsTrue(parsed != null
                          && shooterIds.All(s=>parsed.Data.Any(x=>x.ShooterId == s))
            );
            Assert.AreEqual(shooterIds.Count ,parsed.Data.Count);
        }
        [TestMethod]
        public async Task ShouldCreateShooterSOStagesBeOkHavingProvidedData()
        {
            // get stage without shooterStage
            var existingStage = Scenario.Stages.FirstOrDefault(x=> Scenario.ShooterSOStages.All(s=>s.StageId != x.Id));
            
            if(existingStage== null)
                Assert.Inconclusive("Stage not found");

            var existingMatch = Scenario.Matches.FirstOrDefault(x=> x.Id== existingStage.MatchId);
            if(existingMatch== null)
                Assert.Inconclusive("Match not found");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterSOStages.Count;

            var shooterAssociations = Scenario.ShooterAssociations.Where(x => x.SafetyOfficier && x.AssociationId == existingMatch.AssociationId)
                .Select(x=>x.ShooterId).ToList();

            var stagesInMatch = Scenario.Stages.Where(x => x.MatchId == existingMatch.Id).Select(x => x.Id).ToList();
            
            var existingShooterSo = Scenario.ShooterSOStages.Where(x => stagesInMatch.Contains(x.StageId)).Select(x=>x.ShooterId);

            // not a match director
            var shooterMatches= Scenario.ShooterMatches.Where(x => x.MatchId == existingMatch.Id).Select(x => x.ShooterId).ToList();
            
            var shooters = Scenario.Shooters.Where(x =>
                shooterAssociations.Contains(x.Id) && !shooterMatches.Contains(x.Id) && !existingShooterSo.Contains(x.Id)).ToList();

            if(shooters.Count<2)
                Assert.Inconclusive("Shooters not found");
            
            //Composizione della request
            var request = new ShooterSOStageCreateRequest
            {
                StageId = existingStage.Id,
                Shooters = new List<ShooterSOStageShooterContract>
                {
                    new()
                    {
                        Role = 1,
                        ShooterId = shooters[0].Id
                    },
                    new()
                    {
                        Role = 2,
                        ShooterId = shooters[1].Id
                    }
                }
            };

            //Invoke del metodo
            var response = await Controller.CreateStageSO(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterSOStages.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<ShooterSOStageContract>>(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore + request.Shooters.Count
                          && request.Shooters.All(x =>
                          {
                              var existing = parsed.Data.SingleOrDefault(d => d.Shooter.ShooterId == x.ShooterId);
                              return existing != null && existing.Role == x.Role;
                          })
            );
        }

        [TestMethod]
        public async Task ShouldCreateShooterSOStagesBeOkHavingProvidedDuplicatedData()
        {
            // get stage without shooterStage
            var existingShooterSOStage = Scenario.ShooterSOStages.FirstOrDefault();
            
            if(existingShooterSOStage== null)
                Assert.Inconclusive("Shooter SO Stage not found");
            
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterSOStages.Count;
            
            //Composizione della request
            var request = new ShooterSOStageCreateRequest
            {
                StageId = existingShooterSOStage.StageId,
                Shooters = new List<ShooterSOStageShooterContract>
                {
                    new()
                    {
                        Role = existingShooterSOStage.Role == ShooterRoleEnum.SO ? (int)ShooterRoleEnum.SOChief : (int)ShooterRoleEnum.SO,
                        ShooterId = existingShooterSOStage.ShooterId
                    }
                }
            };

            //Invoke del metodo
            var response = await Controller.CreateStageSO(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterSOStages.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<ShooterSOStageContract>>(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore
                          && parsed.Data.Any(x =>
                          {
                              if (x.Shooter.ShooterId != existingShooterSOStage.ShooterId)
                                  return false;
                              return x.Role ==request.Shooters[0].Role;
                          }));
        }

        [TestMethod]
        public async Task ShouldCreateShooterSOStagesBeBadRequestHavingProvidedDifferentStageData()
        {
            // get stage without shooterStage
            var existingShooterSOStage = Scenario.ShooterSOStages.FirstOrDefault();
            
            if(existingShooterSOStage== null)
                Assert.Inconclusive("Shooter SO Stage not found");
            
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterSOStages.Count;

            var currentStage = Scenario.Stages.FirstOrDefault(x => x.Id == existingShooterSOStage.StageId);
            if(currentStage== null)
                Assert.Inconclusive("Stage not found");

            var anotherStage = Scenario.Stages.FirstOrDefault(x => x.Id != currentStage.Id && x.MatchId == currentStage.MatchId);

            //Composizione della request
            var request = new ShooterSOStageCreateRequest
            {
                StageId = anotherStage.Id,
                Shooters = new List<ShooterSOStageShooterContract>
                {
                    new()
                    {
                        Role = existingShooterSOStage.Role == ShooterRoleEnum.SO ? (int)ShooterRoleEnum.SOChief : (int)ShooterRoleEnum.SO,
                        ShooterId = existingShooterSOStage.ShooterId
                    }
                }
            };

            //Invoke del metodo
            var response = await Controller.CreateStageSO(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterSOStages.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore
                          && parsed.Data.Any());
        }
        
        [TestMethod]
        public async Task ShouldCreateShooterSOStagesBeBadRequestHavingProvidedNotSOShooterData()
        {
            // get stage without shooterStage
            var existingStage = Scenario.Stages.FirstOrDefault(x=> Scenario.ShooterSOStages.All(s=>s.StageId != x.Id));
            
            if(existingStage== null)
                Assert.Inconclusive("Stage not found");

            var existingMatch = Scenario.Matches.FirstOrDefault(x=> x.Id== existingStage.MatchId);
            if(existingMatch== null)
                Assert.Inconclusive("Match not found");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterSOStages.Count;

            var shooterAssociations = Scenario.ShooterAssociations.Where(x => !x.SafetyOfficier && x.AssociationId == existingMatch.AssociationId)
                .Select(x=>x.ShooterId).ToList();

            var stagesInMatch = Scenario.Stages.Where(x => x.MatchId == existingMatch.Id).Select(x => x.Id).ToList();
            
            var existingShooterSo = Scenario.ShooterSOStages.Where(x => stagesInMatch.Contains(x.StageId)).Select(x=>x.ShooterId);

            // not a match director
            var shooterMatches= Scenario.ShooterMatches.Where(x => x.MatchId == existingMatch.Id).Select(x => x.ShooterId).ToList();
            
            var shooter = Scenario.Shooters.FirstOrDefault(x =>
                shooterAssociations.Contains(x.Id) && !shooterMatches.Contains(x.Id) && !existingShooterSo.Contains(x.Id));

            if(shooter == null)
                Assert.Inconclusive("Shooter not found");
            
            //Composizione della request
            var request = new ShooterSOStageCreateRequest
            {
                StageId = existingStage.Id,
                Shooters = new List<ShooterSOStageShooterContract>
                {
                    new()
                    {
                        Role = 1,
                        ShooterId = shooter.Id
                    }
                }
            };

            //Invoke del metodo
            var response = await Controller.CreateStageSO(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterSOStages.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore
                          && request.Shooters.Any()
            );
        }

        [TestMethod]
        public async Task ShouldCreateShooterSOStagesBeBadRequestHavingProvidedNotAssociatedUser()
        {
            // get stage without shooterStage
            var existingStage = Scenario.Stages.FirstOrDefault(x=> Scenario.ShooterSOStages.All(s=>s.StageId != x.Id));
            
            if(existingStage== null)
                Assert.Inconclusive("Stage not found");

            var existingMatch = Scenario.Matches.FirstOrDefault(x=> x.Id== existingStage.MatchId);
            if(existingMatch== null)
                Assert.Inconclusive("Match not found");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterSOStages.Count;

            var shooterAssociations = Scenario.ShooterAssociations.Where(x => x.SafetyOfficier && x.AssociationId == existingMatch.AssociationId)
                .Select(x=>x.ShooterId).ToList();

            var stagesInMatch = Scenario.Stages.Where(x => x.MatchId == existingMatch.Id).Select(x => x.Id).ToList();
            
            var existingShooterSo = Scenario.ShooterSOStages.Where(x => stagesInMatch.Contains(x.StageId)).Select(x=>x.ShooterId);

            // not a match director
            var shooterMatches= Scenario.ShooterMatches.Where(x => x.MatchId == existingMatch.Id).Select(x => x.ShooterId).ToList();
            
            var shooter = Scenario.Shooters.FirstOrDefault(x =>
                shooterAssociations.Contains(x.Id) && !shooterMatches.Contains(x.Id) && !existingShooterSo.Contains(x.Id));

            if(shooter == null)
                Assert.Inconclusive("Shooter not found");

            //look for shooter not in association
            var shooterAssociationId = Scenario.ShooterAssociations.Where(x=>x.SafetyOfficier).GroupBy(x=>x.ShooterId)
                .Where(x => x.All(a=>a.AssociationId != existingMatch.AssociationId))
                .Select(x=>x.Key).FirstOrDefault();

            if(shooterAssociationId == null)
                Assert.Inconclusive("Shooter not in association not found");

            //Composizione della request
            var request = new ShooterSOStageCreateRequest
            {
                StageId = existingStage.Id,
                Shooters = new List<ShooterSOStageShooterContract>
                {
                    new()
                    {
                        Role = 1,
                        ShooterId = shooter.Id
                    },
                    new()
                    {
                        Role = 2,
                        ShooterId = shooterAssociationId
                    }
                }
            };

            //Invoke del metodo
            var response = await Controller.CreateStageSO(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterSOStages.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);

            Assert.IsTrue(parsed != null
                          && parsed.Data.Any());
            // only first shooter is saved
            Assert.AreEqual(countAfter, countBefore + 1);
        }

        [TestMethod]
        public async Task ShouldCreateShooterSOStagesBeBadRequestHavingProvidedExistingShooterMatch()
        {
            // get stage without shooterStage
            var existingStage = Scenario.Stages.FirstOrDefault(x=> Scenario.ShooterSOStages.All(s=>s.StageId != x.Id));
            
            if(existingStage== null)
                Assert.Inconclusive("Stage not found");

            var existingMatch = Scenario.Matches.FirstOrDefault(x=> x.Id== existingStage.MatchId);
            if(existingMatch== null)
                Assert.Inconclusive("Match not found");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterSOStages.Count;

            var shooterAssociations = Scenario.ShooterAssociations.Where(x => x.SafetyOfficier && x.AssociationId == existingMatch.AssociationId)
                .Select(x=>x.ShooterId).ToList();

            // not a match director
            var shooterMatches= Scenario.ShooterMatches.Where(x => x.MatchId == existingMatch.Id).Select(x => x.ShooterId).ToList();

            var matchStageIds = Scenario.Stages.Where(x => x.MatchId == existingMatch.Id).Select(x=>x.Id).ToList();

            var shooterInStages = Scenario.ShooterSOStages.Where(x => matchStageIds.Contains(x.StageId)).Select(x=>x.ShooterId).ToList();

            var shooter = Scenario.Shooters.FirstOrDefault(x =>
                shooterAssociations.Contains(x.Id) && !shooterMatches.Contains(x.Id) && !shooterInStages.Contains(x.Id));

            if(shooter == null)
                Assert.Inconclusive("Shooter not found");

            var existingShooterMatch = Scenario.ShooterMatches.FirstOrDefault(x => x.MatchId == existingMatch.Id);

            if(existingShooterMatch == null)
                Assert.Inconclusive("Shooter match not found");
            
            //Composizione della request
            var request = new ShooterSOStageCreateRequest
            {
                StageId = existingStage.Id,
                Shooters = new List<ShooterSOStageShooterContract>
                {
                    new()
                    {
                        Role = 1,
                        ShooterId = shooter.Id
                    },
                    new()
                    {
                        Role = 2,
                        ShooterId = existingShooterMatch.ShooterId
                    }
                }
            };

            //Invoke del metodo
            var response = await Controller.CreateStageSO(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterSOStages.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null
                          // only first entity is saved
                          && parsed.Data.Any()
            );
            Assert.AreEqual(countAfter,countBefore+1);
        }

        [TestMethod]
        public async Task ShouldDeleteShooterSOStagesBeOkHavingProvidedData()
        {
            // get stage without shooterStage
            var existingShooterSOStage = Scenario.ShooterSOStages.FirstOrDefault();
            
            if(existingShooterSOStage== null)
                Assert.Inconclusive("Shooter SO Stage not found");
            
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterSOStages.Count;
            
            //Composizione della request
            var request = new ShooterSOStageRequest
            {
                ShooterSOStageId = existingShooterSOStage.Id,
            };

            //Invoke del metodo
            var response = await Controller.DeleteStageSO(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterSOStages.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<ShooterSOStageContract>>(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore - 1
                          && parsed.Data.All(x=> x.ShooterSOStageId != request.ShooterSOStageId));
        }
        
        
        [TestMethod]
        public async Task ShouldFetchShooterSOStageBeOkHavingProvidedData()
        {
            var existingStageId = Scenario.ShooterSOStages.GroupBy(x => x.StageId).FirstOrDefault(x => x.Count() > 1)?.Key;
            if (string.IsNullOrEmpty(existingStageId))
                Assert.Inconclusive("Existing stage not found");
            
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterSOStages.Count(x=>x.StageId== existingStageId);
            

            //Composizione della request
            var request = new StageRequest
            {
                StageId = existingStageId,
            };

            //Invoke del metodo
            var response = await Controller.FetchAllShooterSOStages(request);
            
            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<ShooterSOStageContract>>(response);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Count == countBefore
            );
        }
    }
    
    public partial class MatchEntitiesControllerTests
    {
        protected override Shooter GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldFetchAvailableMatchDirectorBeOkHavingProvidedData()
        {
            var existingMatch = Scenario.Matches.FirstOrDefault();
            if(existingMatch== null)
                Assert.Inconclusive("Match not found");
            
            var shooterAssociations = Scenario.ShooterAssociations.Where(x => x.SafetyOfficier && x.AssociationId == existingMatch.AssociationId)
                .Select(x=>x.ShooterId).ToList();

            var shooterMatches= Scenario.ShooterMatches.Where(x => x.MatchId == existingMatch.Id).Select(x => x.ShooterId).ToList();

            var matchStagesIds = Scenario.Stages.Where(x => x.MatchId == existingMatch.Id).Select(x => x.Id).ToList();

            var shooterSO = Scenario.ShooterSOStages.Where(x => matchStagesIds.Contains(x.StageId)).Select(x=>x.ShooterId).ToList();
            
            var shooterIds = Scenario.Shooters.Where(x =>
                shooterAssociations.Contains(x.Id) && !shooterMatches.Contains(x.Id) && !shooterSO.Contains(x.Id)).Select(x=>x.Id).ToList();

            if(shooterIds.Count== 0)
                Assert.Inconclusive("Shooters not found");

            //Composizione della request
            var request = new MatchRequest()
            {
                MatchId = existingMatch.Id
            };

            //Invoke del metodo
            var response = await Controller.FetchAvailableMatchDirector(request);
            
            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<ShooterContract>>(response);
            Assert.IsTrue(parsed != null
                          && shooterIds.All(s=>parsed.Data.Any(x=>x.ShooterId == s))
            );
            Assert.AreEqual(shooterIds.Count ,parsed.Data.Count);
        }

        [TestMethod]
        public async Task ShouldCreateMatchDirectorBeOkHavingProvidedData()
        {
            var existingMatch = Scenario.Matches.FirstOrDefault();
            if(existingMatch== null)
                Assert.Inconclusive("Match not found");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterMatches.Count;

            var shooterAssociations = Scenario.ShooterAssociations.Where(x => x.SafetyOfficier && x.AssociationId == existingMatch.AssociationId)
                .Select(x=>x.ShooterId).ToList();

            var shooterMatches= Scenario.ShooterMatches.Where(x => x.MatchId == existingMatch.Id).Select(x => x.ShooterId).ToList();

            var matchStagesIds = Scenario.Stages.Where(x => x.MatchId == existingMatch.Id).Select(x => x.Id).ToList();

            var shooterSO = Scenario.ShooterSOStages.Where(x => matchStagesIds.Contains(x.StageId)).Select(x=>x.ShooterId).ToList();
            
            var shooter = Scenario.Shooters.FirstOrDefault(x =>
                shooterAssociations.Contains(x.Id) && !shooterMatches.Contains(x.Id) && !shooterSO.Contains(x.Id));

            if(shooter== null)
                Assert.Inconclusive("Shooter not found");

            //Composizione della request
            var request = new ShooterMatchCreateRequest
            {
                MatchId = existingMatch.Id,
                ShooterIds = new List<string>{shooter.Id}
            };

            //Invoke del metodo
            var response = await Controller.CreateMatchDirector(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterMatches.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<ShooterMatchContract>>(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore + request.ShooterIds.Count
                          && parsed.Data.Any(x=>x.Shooter.ShooterId == shooter.Id)
            );
        }

        [TestMethod]
        public async Task ShouldCreateMatchDirectorBeOkHavingProvidedDuplicatedData()
        {
            var existingMatch = Scenario.Matches.FirstOrDefault();
            if(existingMatch== null)
                Assert.Inconclusive("Match not found");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterMatches.Count;

            var shooterAssociations = Scenario.ShooterAssociations.Where(x => x.AssociationId == existingMatch.AssociationId && x.SafetyOfficier)
                .Select(x=>x.ShooterId).ToList();

            var shooterMatches= Scenario.ShooterMatches.Where(x => x.MatchId == existingMatch.Id).Select(x => x.ShooterId).ToList();
            
            var shooter = Scenario.Shooters.FirstOrDefault(x =>
                shooterAssociations.Contains(x.Id) && shooterMatches.Contains(x.Id));

            if(shooter== null)
                Assert.Inconclusive("Shooter not found");

            //Composizione della request
            var request = new ShooterMatchCreateRequest
            {
                MatchId = existingMatch.Id,
                ShooterIds = new List<string>{shooter.Id}
            };

            //Invoke del metodo
            var response = await Controller.CreateMatchDirector(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterMatches.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<ShooterMatchContract>>(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore
                          && parsed.Data.Any(x=>x.Shooter.ShooterId == shooter.Id)
            );
        }

        [TestMethod]
        public async Task ShouldCreateMatchDirectorBeRequestHavingProvidedNoSOShooterData()
        {
            var existingMatch = Scenario.Matches.FirstOrDefault();
            if(existingMatch== null)
                Assert.Inconclusive("Match not found");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterMatches.Count;

            var shooterAssociations = Scenario.ShooterAssociations.Where(x => !x.SafetyOfficier && x.AssociationId == existingMatch.AssociationId)
                .Select(x=>x.ShooterId).ToList();

            var shooterMatches= Scenario.ShooterMatches.Where(x => x.MatchId == existingMatch.Id).Select(x => x.ShooterId).ToList();

            var matchStagesIds = Scenario.Stages.Where(x => x.MatchId == existingMatch.Id).Select(x => x.Id).ToList();

            var shooterSO = Scenario.ShooterSOStages.Where(x => matchStagesIds.Contains(x.StageId)).Select(x=>x.ShooterId).ToList();
            
            var shooter = Scenario.Shooters.FirstOrDefault(x =>
                shooterAssociations.Contains(x.Id) && !shooterMatches.Contains(x.Id) && !shooterSO.Contains(x.Id));

            if(shooter== null)
                Assert.Inconclusive("Shooter not found");

            //Composizione della request
            var request = new ShooterMatchCreateRequest
            {
                MatchId = existingMatch.Id,
                ShooterIds = new List<string>{shooter.Id}
            };

            //Invoke del metodo
            var response = await Controller.CreateMatchDirector(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterMatches.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore
                          && parsed.Data.Any()
            );
        }

        [TestMethod]
        public async Task ShouldCreateMatchDirectorBeBadRequestHavingProvidedNotAssociation()
        {
            var existingMatch = Scenario.Matches.FirstOrDefault();
            if(existingMatch== null)
                Assert.Inconclusive("Match not found");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterMatches.Count;

            //look for shooter not in association
            var shooterAssociations = Scenario.ShooterAssociations.Where(x=>x.SafetyOfficier).GroupBy(x=>x.ShooterId)
                .Where(x => x.All(a=>a.AssociationId != existingMatch.AssociationId))
                .Select(x=>x.Key).ToList();

            var shooterMatches= Scenario.ShooterMatches.Where(x => x.MatchId == existingMatch.Id).Select(x => x.ShooterId).ToList();
            
            var shooter = Scenario.Shooters.FirstOrDefault(x =>
                shooterAssociations.Contains(x.Id) && !shooterMatches.Contains(x.Id));

            if(shooter== null)
                Assert.Inconclusive("Shooter not found");

            //Composizione della request
            var request = new ShooterMatchCreateRequest
            {
                MatchId = existingMatch.Id,
                ShooterIds = new List<string>{shooter.Id}
            };

            //Invoke del metodo
            var response = await Controller.CreateMatchDirector(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterMatches.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore
                          && parsed.Data.Count > 0
            );
        }
        [TestMethod]
        public async Task ShouldCreateMatchDirectorBeBadRequestHavingProvidedExistingShooterSOStageData()
        {
            var existingMatch = Scenario.Matches.FirstOrDefault();
            if(existingMatch== null)
                Assert.Inconclusive("Match not found");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterMatches.Count;

            var shooterAssociations = Scenario.ShooterAssociations.Where(x => x.SafetyOfficier && x.AssociationId == existingMatch.AssociationId)
                .Select(x=>x.ShooterId).ToList();

            var shooterMatches= Scenario.ShooterMatches.Where(x => x.MatchId == existingMatch.Id).Select(x => x.ShooterId).ToList();
            
            var shooter = Scenario.Shooters.FirstOrDefault(x =>
                shooterAssociations.Contains(x.Id) && shooterMatches.Contains(x.Id));

            if(shooter== null)
                Assert.Inconclusive("Shooter not found");

            var matchStages = Scenario.Stages.Where(x => x.MatchId == existingMatch.Id).Select(x=>x.Id).ToList();

            var shooterSO = Scenario.ShooterSOStages.FirstOrDefault(x => matchStages.Contains(x.StageId));

            if(shooterSO == null)
                Assert.Inconclusive("Shooter SO not found");

            //Composizione della request
            var request = new ShooterMatchCreateRequest
            {
                MatchId = existingMatch.Id,
                ShooterIds = new List<string>{shooter.Id,shooterSO.ShooterId}
            };

            //Invoke del metodo
            var response = await Controller.CreateMatchDirector(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterMatches.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore
                          && parsed.Data.Any()
            );
        }

        [TestMethod]
        public async Task ShouldDeleteMatchDirectorBeOkHavingProvidedData()
        {
            var existing = Scenario.ShooterMatches.FirstOrDefault();
            if(existing == null)
                Assert.Inconclusive("Shooter match not found");
            var countBefore = Scenario.ShooterMatches.Count;


            //Composizione della request
            var request = new ShooterMatchRequest
            {
                ShooterMatchId = existing.Id
            };

            //Invoke del metodo
            var response = await Controller.DeleteMatchDirector(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterMatches.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<ShooterMatchContract>>(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore -1
                          && parsed.Data.All(x=>x.ShooterMatchId != request.ShooterMatchId)
            );
        }

        [TestMethod]
        public async Task ShouldFetchMatchDirectorBeOkHavingProvidedData()
        {
            var existingMatch = Scenario.Matches.FirstOrDefault();
            if(existingMatch== null)
                Assert.Inconclusive("Match not found");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterMatches.Count(x=>x.MatchId== existingMatch.Id);
            

            //Composizione della request
            var request = new MatchRequest
            {
                MatchId = existingMatch.Id,
            };

            //Invoke del metodo
            var response = await Controller.FetchAllMatchDirector(request);
            
            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<ShooterMatchContract>>(response);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Count == countBefore);
        }
    }
}