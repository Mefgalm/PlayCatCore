﻿using System.Linq;
using System;
using PlayCat.DataService.DTO;
using PlayCat.DataService.Mappers;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlayCat.DataService.Response;
using PlayCat.DataService.Request;

namespace PlayCat.DataService
{
    public class AudioService : BaseService, IAudioService
    {        
        public AudioService(PlayCatDbContext dbContext, 
            ILoggerFactory loggerFactory)
            : base(dbContext, loggerFactory.CreateLogger<AuthService>())
        {
        }    
        
        public AudioResult SearchAudios(string searchString, int skip, int take)
        {
            return BaseInvoke(() =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return ResponseBuilder<AudioResult>.SuccessBuild(new AudioResult { Audios = Enumerable.Empty<ApiModel.Audio>() });

                searchString = searchString.Trim();

                int excludeMarker = int.MinValue;

                IEnumerable<DataModel.Audio> audios =
                   (from a in _dbContext.Audios
                    select new
                    {
                        Audio = a,
                        Rank = a.UniqueIdentifier == searchString ? 1 :
                               a.Song.StartsWith(searchString) ? 2 :
                               a.Artist.StartsWith(searchString) ? 3 :
                               (a.Artist + " " + a.Song).StartsWith(searchString) ? 4 : excludeMarker
                    } into a
                    where a.Rank != excludeMarker
                    orderby a.Rank
                    select a.Audio)
                    .Skip(skip)
                    .Take(take)
                    .ToList();

                return ResponseBuilder<AudioResult>.SuccessBuild(new AudioResult
                {
                    Audios = audios.Select(x => AudioMapper.ToApi.FromData(x))
                });
            });
        }

        public BaseResult RemoveFromPlaylist(Guid userId, AddRemovePlaylistRequest request)
        {
            return BaseInvoke(() =>
            {
                //Is need check audioId??? - perfomance issue
                var playlistInfo =
                    (from p in _dbContext.Playlists.Include(x => x.AudioPlaylists)
                     where p.Id == request.PlaylistId && p.OwnerId == userId
                     select new
                     {
                         Playlist = p,
                         AddedAudioPlaylist = p.AudioPlaylists.FirstOrDefault(x => x.AudioId == request.AudioId)
                     })
                    .FirstOrDefault();

                if (playlistInfo == null)
                    return ResponseBuilder<BaseResult>.Fail().SetInfoAndBuild("Playlist not found");

                if (playlistInfo.AddedAudioPlaylist != null)
                {
                    _dbContext.AudioPlaylists.Remove(playlistInfo.AddedAudioPlaylist);
                    _dbContext.SaveChanges();
                }                    

                return ResponseBuilder<BaseResult>.SuccessBuild();
            });
        }

        public BaseResult AddToPlaylist(Guid userId, AddRemovePlaylistRequest request)
        {
            return BaseInvokeWithTransaction(() =>
            {
                //Is need check audioId??? - perfomance issue
                var playlistInfo =
                    (from p in _dbContext.Playlists.Include(x => x.AudioPlaylists)
                    where p.Id == request.PlaylistId && p.OwnerId == userId
                    select new
                    {
                        Playlist = p,
                        IsAlreadyAdded = p.AudioPlaylists.Any(x => x.AudioId == request.AudioId)
                    })
                    .FirstOrDefault();

                if (playlistInfo == null)
                    return ResponseBuilder<BaseResult>.Fail().SetInfoAndBuild("Playlist not found");

                if (playlistInfo.IsAlreadyAdded)
                    return ResponseBuilder<BaseResult>.Fail().SetInfoAndBuild("Audio is already added");

                var audioPlaylist = new DataModel.AudioPlaylist
                {
                    AudioId = request.AudioId,
                    DateCreated = DateTime.Now,
                    Order = playlistInfo.Playlist.OrderValue,
                    PlaylistId = request.PlaylistId                    
                };

                playlistInfo.Playlist.OrderValue++;
                _dbContext.Entry(playlistInfo.Playlist).State = EntityState.Modified;

                _dbContext.AudioPlaylists.Add(audioPlaylist);
                _dbContext.SaveChanges();

                return ResponseBuilder<BaseResult>.SuccessBuild();
            });
        }

        public AudioResult GetAudios(Guid playlistId, int skip, int take)
        {
            return BaseInvoke(() =>
            {
                //get audios with paging
                IEnumerable<ApiModel.Audio> apiAudios =
                    (from ap in _dbContext.AudioPlaylists
                     join a in _dbContext.Audios on ap.AudioId equals a.Id
                     where ap.PlaylistId == playlistId
                     orderby ap.Order descending
                     select new AudioDTO
                     {
                         Id = a.Id,
                         AccessUrl = a.AccessUrl,
                         Artist = a.Artist,
                         DateAdded = ap.DateCreated,
                         Song = a.Song,
                         Uploader = a.Uploader
                     })
                    .Skip(skip)
                    .Take(take)
                    .ToList()
                    .Select(x => AudioMapper.ToApi.FromDTO(x));

                return ResponseBuilder<AudioResult>.SuccessBuild(new AudioResult
                {
                    Audios = apiAudios
                });
            });
        }                
    }
}
