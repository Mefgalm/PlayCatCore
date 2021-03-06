﻿using System.Collections.Generic;
using System.Linq;
using PlayCat.ApiModels;

namespace PlayCat.DataServices.Response.AudioResponse
{
    public class AudioResult : BaseResult
    {
        public AudioResult() : base(new BaseResult())
        {
        }

        public AudioResult(BaseResult baseResult) : base(baseResult)
        {
        }

        public IEnumerable<Audio> Audios { get; set; } = Enumerable.Empty<Audio>();
    }
}
