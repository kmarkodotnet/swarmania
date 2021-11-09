
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Horde : Agent
{
    static int counter = 0;
    ConcurrentDictionary<int, HordeMember> _members { get; set; }
    public Horde _enemyHorde;
    public static List<BugTypeEnum> _species { get; set; }

    private GameObject _initGameObject;
    [SerializeField] string myTeamId;
    [SerializeField] int teamCount;
    [SerializeField] int hordeId;

    List<float> friendsWeights, enemiesWeights;
    private HordeStateEnum state;

    bool _attack = false;
    public override void Initialize()
    {
        hordeId = counter;
        counter++;
        _species = new List<BugTypeEnum> { BugTypeEnum.AntSlave, BugTypeEnum.AntWarrior, BugTypeEnum.AntFlyer, BugTypeEnum.Ladybug, BugTypeEnum.Termite1 };
    }

    #region Agent
    public void Reset()
    {
        //SetState(HordeStateEnum.Created);
        //Log($"created initalize");
        //if (_hordeClients == null)
        //    _hordeClients = new ConcurrentDictionary<int, HordeClient>();
    }

    public void SetAgentModel(string name, NNModel model)
    {
        Log((name).ToString());
        SetModel(name, model);
    }
    public override void OnEpisodeBegin()
    {
        Reset();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        if(state != HordeStateEnum.Created)
        {
            //Debug.Log(state);
        }

        if (state == HordeStateEnum.RequestingDecision && _enemyHorde) { 
            friendsWeights = GetWeightOfSpecies();
            AddObservations(sensor, friendsWeights, enemiesWeights);
        }
        else
        {
            Debug.LogWarning($"collecting observations but not in REQUESTING state but in {state}");

            friendsWeights = GetWeightOfSpecies();
            if(_enemyHorde == null)
            {
                enemiesWeights = GetZeroWeightOfSpecies();
            }
            else
            {
                enemiesWeights = _enemyHorde.GetWeightOfSpecies();
            }            
            AddObservations(sensor, friendsWeights, enemiesWeights);
        }
    }

    private void AddObservations(VectorSensor sensor, List<float> friendsWeights, List<float> enemiesWeights)
    {
        var minValue = 0;
        var maxValue = 19;
        var sb = new System.Text.StringBuilder();
        foreach (var friendsWeight in friendsWeights)
        {
            var normalizedFriendsWeight = (friendsWeight - minValue) / (maxValue - minValue);
            sb.Append(" " + friendsWeight);
            sensor.AddObservation(normalizedFriendsWeight);
        }
        sb.Append("  -  ");
        foreach (var enemiesWeight in enemiesWeights)
        {
            var normalizedFriendsWeight = (enemiesWeight - minValue) / (maxValue - minValue);
            sb.Append(" " + enemiesWeight);
            sensor.AddObservation(normalizedFriendsWeight);
        }
        Debug.LogWarning(sb.ToString());
    }

    private string GetWeightsAsString()
    {
        var sb = new System.Text.StringBuilder();
        foreach (var friendsWeight in friendsWeights)
        {
            sb.Append(" " + friendsWeight);
        }
        sb.Append("  -  ");
        foreach (var enemiesWeight in enemiesWeights)
        {
            sb.Append(" " + enemiesWeight);
        }
        return sb.ToString();
    }

    private string GetCumulativeWeights()
    {
        float wFriend = GetCumulativeWeight(friendsWeights);
        float wEnemy = GetCumulativeWeight(enemiesWeights);
        return $"<{wFriend} - {wEnemy}>";
    }

    private float GetCumulativeWeight(List<float> weights)
    {
        float w = 0f;
        for (int i = 0; i < _species.Count; i++)
        {
            var bugType = _species[i];
            var wi = weights[i];
            switch (bugType)
            {
                case BugTypeEnum.AntSlave:
                    w += wi * 1.1f;
                    break;
                case BugTypeEnum.AntWarrior:
                    w += wi * 1.9f;
                    break;
                case BugTypeEnum.AntFlyer:
                    w += wi * 1.7f;
                    break;
                case BugTypeEnum.Termite1:
                    w += wi * 1f;
                    break;
                case BugTypeEnum.Ladybug:
                    w += wi * 2.0f;
                    break;
                default:
                    throw new System.Exception("unknown bugtype");
            }
        }
        return w;
    }

    private void FindEnemyHorde()
    {
        GameObject nearestEnemy = null;
        _enemyHorde = null;

        var friendsAlive = Members().Where(member => member.GetComponent<BugStateHandler>().GetState() != BugStateEnum.Dead);
        foreach (var friend in friendsAlive)
        {
            if(nearestEnemy == null)
            {
                nearestEnemy = FindNearestEnemy(friend);
            }
            if(_enemyHorde == null && nearestEnemy)
            {
                _enemyHorde = FindObjectOfType<EntityCollection>().GetMyHorde(nearestEnemy);
                if (_enemyHorde)
                {
                    enemiesWeights = _enemyHorde.GetWeightOfSpecies();
                }
            }
        }
    }

    private void Log(string temaplte)
    {
        var item = Members().FirstOrDefault();
        if (item)
        {
            myTeamId = item.GetComponent<SelectableObject>().GetOwnerId();
        }
        //Debug.Log($"TEAM {myTeamId} : HORDE {hordeId} -> {temaplte} HORDE");
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if(state == HordeStateEnum.RequestingDecision)
        {
            var attack = actionBuffers.ContinuousActions[0];
            var runaway = actionBuffers.ContinuousActions[1];
            _attack = attack > runaway;

            Log();
            Debug.Log("x");
            SetState(HordeStateEnum.DecisionArrived);
        }
        else
        {
            Debug.LogWarning($"action recieved but not in DECISION state but in {state}");
        }
               
    }
    public BugTypeEnum GetHordeBugType()
    {
        return Members().First().GetComponent<AttackableBug>().GetBugType();
    }
    private void Log()
    {
        var myTeamWeight = GetWeightSum();
        var myReward = GetCumulativeReward();
        var myBugType = GetHordeBugType();
        var enemyTeamWeight = 0f;
        var enemyReward = 0f;
        BugTypeEnum? enemyBugType = null;
        if (_enemyHorde)
        {
            enemyTeamWeight = _enemyHorde.GetWeightSum();
            enemyReward = _enemyHorde.GetCumulativeReward();
            enemyBugType = _enemyHorde.GetHordeBugType();
        }
        var myBugTypeText = myBugType.ToString();
        var enemyBugTypeText = enemyBugType == null ? "---" : enemyBugType.ToString();
        var x = GetWeightsAsString();
        var y = GetCumulativeWeights();
        if (_attack)
        {
            if(myTeamWeight > enemyTeamWeight * 1.2f)
            {
                Debug.Log($"{myTeamId}:{x} - {y} {myTeamWeight} -> {enemyTeamWeight}");
            }else if(myTeamWeight <= enemyTeamWeight * 1.2f && myTeamWeight * 1.2f > enemyTeamWeight)
            {
                Debug.Log($"{myTeamId}:{x} - {y}            {myTeamWeight} -> {enemyTeamWeight}");
            }else if(myTeamWeight * 1.2f <= enemyTeamWeight)
            {
                Debug.Log($"{myTeamId}:{x} - {y}                         {myTeamWeight} -> {enemyTeamWeight}");
            }
        }
        else
        {
            if (myTeamWeight > enemyTeamWeight * 1.2f)
            {
                Debug.LogError($"{myTeamId}:{x} - {y}                         {myTeamWeight} -> {enemyTeamWeight}");
            }
            else if (myTeamWeight <= enemyTeamWeight * 1.2f && myTeamWeight * 1.2f > enemyTeamWeight)
            {
                Debug.LogError($"{myTeamId}:{x} - {y}             {myTeamWeight} -> {enemyTeamWeight}");
            }
            else if (myTeamWeight * 1.2f <= enemyTeamWeight)
            {
                Debug.LogError($"{myTeamId}:{x} - {y} {myTeamWeight} -> {enemyTeamWeight}");
            }
        }
    }
    #endregion

    private void Update()
    {
        //Debug.Log($"{FindObjectOfType<Breeder>().GetCurrentBreed()} : {hordeId} -> {GetCumulativeReward()}");
        RewardAlives();
        HandleStates();
        //TODO Actualize horde members, if needed, create new
    }

    private void RewardAlives()
    {
        var aliveBugs = GetAliveSum();
        var allBugs = GetWeightSum();
        
        if(allBugs <= 0 || aliveBugs <= 0)
            return;
        
        //AddReward(2*aliveBugs / allBugs);
    }

    private void HandleStates()
    {
        if (state == HordeStateEnum.Created)
        {

        }
        else if(state == HordeStateEnum.Idle)
        {
            teamCount = Members().Count;
            //
            FindEnemyHorde();
            if (_enemyHorde)
            {
                RequestDecision();
                SetState(HordeStateEnum.RequestingDecision);
            }
        }
        else if(state == HordeStateEnum.DecisionArrived)
        {
            AttackOrRunaway();
        }else if(state == HordeStateEnum.InProgressDecision)
        {
            CheckIfFinished();
        }
        else if(state == HordeStateEnum.Finished)
        {
            //SetState(HordeStateEnum.Idle);
        }
    }

    private void SetState(HordeStateEnum newState)
    {
        //Debug.LogWarning($"{state} -> {newState}");
        state = newState;
    }

    private void CheckIfFinished()
    {
        var finishedStates = new List<BugStateEnum> { BugStateEnum.Idle, BugStateEnum.Dead };
        
        if(Members().All(member => finishedStates.Contains(member.GetComponent<BugStateHandler>().GetState())))
        {
            SetState(HordeStateEnum.Finished);
        }
    }

    private void AttackOrRunaway()
    {
        if (_attack)
        {
            Attack(_enemyHorde);
        }
        else
        {
            RunAway(_enemyHorde);
        }
        SetState(HordeStateEnum.InProgressDecision);
    }

    #region List methods
    public bool ContainsBug(GameObject bug)
    {
        if (_members == null || bug == null)
        {
            return false;
        }
        return _members.ContainsKey(bug.GetComponent<Bug>().GetId());
    }
    private void Add(GameObject bug)
    {
        if(_members == null)
            _members = new ConcurrentDictionary<int, HordeMember>();

        if (!_members.ContainsKey(bug.GetComponent<Bug>().GetId()))
        {
            if (!_members.TryAdd(bug.GetComponent<Bug>().GetId(), new HordeMember { Bug = bug, State = HordeClientStateEnum.Inserted }))
            {
                Log($"{bug.GetComponent<Bug>().GetId()} - failed to add to horde");
            }
        }
    }

    public void Remove(GameObject bug)
    {
        HordeMember ro;
        var id = bug.GetComponent<Bug>().GetId();
        if (!_members.TryRemove(id, out ro) || ro == null)
        {
            Log($"{id} failed to remove");
        }
    }

    #endregion

    #region Group methods

    private GameObject FindNearestEnemy(GameObject bug)
    {
        var lookDistance = bug.GetComponent<LivingObject>().GetLookDistance();
        var nearUnits = Physics2D.OverlapCircleAll(new Vector2(bug.transform.position.x, bug.transform.position.y), lookDistance)
            .Where(u => u.tag == "bug");
        GameObject nearestEnemy = null;
        float nearestDistance = 0;
        foreach (var unit in nearUnits.Where(nu => nu.GetComponent<BugStateHandler>().GetState() != BugStateEnum.Dead))
        {
            bool areEnemies = AreEnemies(bug, unit.gameObject);
            //Debug.Log($"Me: {gameObject.name} other: {unit.gameObject.name} enemy: {isEnemy}");
            if (areEnemies)
            {
                var myCollider = bug.GetComponent<CapsuleCollider2D>();
                var otherCollider = unit;
                var currentDistance = Physics2D.Distance(myCollider, otherCollider).distance;
                if (nearestEnemy == null || nearestDistance > currentDistance)
                {
                    nearestEnemy = unit.gameObject;
                    nearestDistance = currentDistance;
                }
            }
        }
        //Debug.Log($"$$$$$$$$$$$$  Me: {gameObject.name} other: {nearestEnemy.name}");
        return nearestEnemy;
    }

    public void Attack(Horde enemy)
    {
        var enemies = enemy.Members();
        var friends = Members();
        foreach (var friend in friends)
        {
            var enemyToAttack = CalculateEnemyToAttack(friend, enemies);
            Log($"attack enemy -> {enemyToAttack}");
            AttackEnemy(friend, enemyToAttack);
        }
    }

    public void RunAway(Horde enemy)
    {
        var friends = Members();
        var myCenter = FindHordeCenter();
        var enemyCenter = enemy.FindHordeCenter();
        var diff = myCenter - enemyCenter;
        foreach (var friend in friends)
        {
            Log($"runaway to diff -> {diff}");
            friend.GetComponent<BugActionSet>().RunAway(diff);
        }
    }

    private GameObject CalculateEnemyToAttack(GameObject friend, List<GameObject> enemies)
    {
        var myCollider = friend.GetComponent<CapsuleCollider2D>();

        GameObject nearestEnemy = null;
        float nearestDistance = 0;
        var orderedEnemies = enemies.OrderByDescending(e =>
        {
            var otherCollider = e.GetComponent<CapsuleCollider2D>();
            var currentDistance = Physics2D.Distance(myCollider, otherCollider).distance;
            return currentDistance;
        });
        var top = 3;
        var enemiesCount = orderedEnemies.Count();
        if(enemiesCount <= 0)
        {
            return null;
        }
        var max = System.Math.Min(enemiesCount, top);
        var topEnemies = orderedEnemies.Take(max);
        var randomEnemy = topEnemies.ToArray()[UnityEngine.Random.Range(0, max)];
        return randomEnemy;
    }

    public GameObject GetNearestEnemy()
    {
        var nes = new List<GameObject>();
        foreach (var bug in _members)
        {
            var ne = bug.Value.Bug.GetComponent<BugActionSet>().FindNearestEnemy();
            if (ne != null)
            {
                nes.Add(ne);
            }
        }
        if (nes.Count == 0)
        {
            return null;
        }
        var hordeCenter = FindHordeCenter();
        var nearest = nes
            .Select(ne => new { Enemy = ne, Distance = ne.transform.position - hordeCenter })
            .OrderBy(ned => ned.Distance.magnitude)
            .First().Enemy;

        return nearest;
    }

    
    #endregion

    public void Lookup(GameObject bug, string teamId, bool root)
    {
        teamId = bug.GetComponent<SelectableObject>().GetOwnerId();
        if (bug == null)
            return;
        var livingObject = bug.GetComponent<LivingObject>();
        if (livingObject)
        {
            var lookDistance = livingObject.GetLookDistance();
            var nearUnits = Physics2D.OverlapCircleAll(new Vector2(bug.transform.position.x, bug.transform.position.y), lookDistance)
                .Where(n => n.tag == "bug" && n.GetComponent<Attack>() && n.GetComponent<BugStateHandler>().GetState() != BugStateEnum.Dead)
                .Select(n => n.gameObject).ToList();


            nearUnits = nearUnits.Where(n => n.GetComponent<SelectableObject>().GetOwnerId() == teamId).ToList();

            foreach (var nearUnit in nearUnits)
            {
                Add(nearUnit);
            }
            var ms = Members();
            if (_members.ContainsKey(bug.GetComponent<Bug>().GetId()))
                _members[bug.GetComponent<Bug>().GetId()].State = HordeClientStateEnum.LookedUp;

            var unitsToLokup = _members.Where(k => k.Value.State == HordeClientStateEnum.Inserted).Select(s => s.Key).ToList();
            foreach (var item in unitsToLokup)
            {
                Lookup(_members[item].Bug, teamId, false);
            }
        }
        if (root)
        {
            Log($"BUG ROOT {bug.GetComponent<Bug>().GetId()} - teamid {teamId} ");
            SetState(HordeStateEnum.Idle);
        }
        //Debug.LogWarning($"{_initGameObject.GetComponent<SelectableObject>().GetOwnerId()}:{_initGameObject.GetComponent<Bug>().GetId()} - RIND CREATED: {_hordeObjects.Count}");
    }


    internal void AwardBeforeEndEpisode()
    {
        var myTeamAllWeight = GetWeightSum();
        var myTeamAliveWeight = GetAliveSum();
        var enemyTeamWeight = 0f;
        var enemyTeamAliveWeight = 0f;
        if (_enemyHorde)
        {
            enemyTeamWeight = _enemyHorde.GetWeightSum();
            enemyTeamAliveWeight = _enemyHorde.GetAliveSum(); 
            
            if (_attack)
            {
                Debug.LogWarning($"{myTeamAliveWeight} / {myTeamAllWeight} >= {enemyTeamAliveWeight} / {enemyTeamWeight}");
                if (myTeamAliveWeight / myTeamAllWeight >= enemyTeamAliveWeight / enemyTeamWeight)
                {
                    AddReward(myTeamAliveWeight / myTeamAllWeight - enemyTeamAliveWeight / enemyTeamWeight);
                }
                else
                {
                    AddReward(myTeamAliveWeight / myTeamAllWeight- enemyTeamAliveWeight / enemyTeamWeight);
                }
            }
            else
            {
                AddReward(0.25f);
            }
        }
        
    }


    #region Weigths
    public static List<BugTypeEnum> GetSpecies()
    {
        var bts = new List<BugTypeEnum>();
        foreach (BugTypeEnum bt in (BugTypeEnum[])System.Enum.GetValues(typeof(BugTypeEnum)))
        {
            bts.Add(bt);
        }
        return bts;
    }

    public List<float> GetWeightOfSpecies()
    {
        var weightOfSpecies = new List<float>();
        foreach (var bugtype in _species)
        {
            var bugTypeCount = Members().Where(ro => ro.GetComponent<AttackableBug>().GetBugType() == bugtype).Count();
            weightOfSpecies.Add(bugTypeCount);
        }
        return weightOfSpecies;
    }
    public List<float> GetZeroWeightOfSpecies()
    {
        var weightOfSpecies = new List<float>();
        foreach (var bugtype in _species)
        {
            weightOfSpecies.Add(0);
        }
        return weightOfSpecies;
    }
    public float GetWeightSum()
    {
        if(_members == null)
        {
            return 0;
        }
        return _members.Count;
    }
    public float GetAliveSum()
    {
        if (_members == null || _members.Count <= 0)
            return 0;
        //Where(ro => ro.Value.Bug != null).
        return Members().Count(ro => ro.GetComponent<BugStateHandler>().GetState() != BugStateEnum.Dead);
    }
    #endregion

    #region Helpers
    public List<GameObject> Members()
    {
        if(_members == null)
        {
            return new List<GameObject>();
        }
        var ms = new ConcurrentDictionary<int, HordeMember>();
        foreach (var item in _members)
        {
            if(item.Value.Bug != null)
            {
                ms.TryAdd(item.Key, item.Value);
            }
        }
        _members = ms;
        return _members.Select(r => r.Value.Bug).ToList();
    }
    private Vector3 FindHordeCenter()
    {
        var ms = _members;//.Where(m => m.Value.Bug != null);
        return new Vector3(ms.Select(b => b.Value.Bug.transform.position.x).Average(), ms.Select(b => b.Value.Bug.transform.position.y).Average(), ms.Select(b => b.Value.Bug.transform.position.x).Average());
    }

    private void AttackEnemy(GameObject friend, GameObject enemyToAttack)
    {
        Debug.Log("y");
        friend.GetComponent<BugActionSet>().MoveToAttack(enemyToAttack);
    }

    private bool AreEnemies(GameObject go1, GameObject go2)
    {
        if (!go1.GetComponent<SelectableObject>() || !go2.GetComponent<SelectableObject>())
        {
            return false;
        }
        var o1 = go1.GetComponent<SelectableObject>().GetOwnerId();
        var o2 = go2.GetComponent<SelectableObject>().GetOwnerId();
        return o1 != o2;
    }
    #endregion

}