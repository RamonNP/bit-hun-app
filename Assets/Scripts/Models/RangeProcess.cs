using System;
using System.Collections.Generic;

namespace GoldenRaspberry.Models
{
    [Serializable]
    public class RangeProcess
    {
        public int id;
        public string userToken;
        public string initialRange;
        public string finalRange;
        public int quantity;
        public string hashValidate;
        public List<Puzzle> puzzles;
    }

    [Serializable]
    public class Puzzle
    {
        public int id;
        public string privateKeyRange;
        public string privateKeyPublicKey;
        public string bitcoinAddress;
        public bool status;
    }

    [Serializable]
    public class NoWinDataPayload
    {
        public int Id { get; set; }
        public string UserToken { get; set; }
        public string InitialRange { get; set; }
        public string FinalRange { get; set; }
        public string HashValidate { get; set; }
        public int Quantity { get; set; }
    }
}
