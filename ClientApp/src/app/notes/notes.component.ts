import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Note } from '../models/Note.model';

@Component({
  selector: 'app-notes',
  templateUrl: './notes.component.html',
  styleUrls: ['./notes.component.css']
})
export class NotesComponent implements OnInit {
  userNotes: Note[] = [];
  publicNotes: Note[] = [];
  allNotes: Note[] = [];
  decryptedNote: Note = new Note();
  addNewMode: boolean = false;
  showNoteMode: boolean = false;
  choosedNote: number = -1;
  addNewError: boolean = false;
  publicChecked: boolean = false;
  encryptedChecked: boolean = false;
  publicNotesMode: boolean = false;
  
  constructor(private router: Router, private http: HttpClient) { }

  async ngOnInit() {
    await this.loadNotes();
  }

  async loadNotes(){
      this.userNotes = await this.http.get<Note[]>("http://localhost:8383/api/notes/usernotes/").toPromise();

      this.publicNotes = await this.http.get<Note[]>("http://localhost:8383/api/notes/public/").toPromise();

      let flag = false;
      for(let i=0;i<this.userNotes.length;i++){
        flag = false;
        for(let j=0;j<this.allNotes.length;j++){
          if(this.allNotes[j].id == this.userNotes[i].id){
            flag = true;
          }
        }
        if(flag == false){
          this.allNotes.push(this.userNotes[i]);
        }
      }

      for(let i=0;i<this.publicNotes.length;i++){
        flag = false;
        for(let j=0;j<this.allNotes.length;j++){
          if(this.allNotes[j].id == this.publicNotes[i].id){
            flag = true;
          }
        }
        if(flag == false){
          this.allNotes.push(this.publicNotes[i]);
        }
      }
  }

  showNote(id: number){
    this.showNoteMode = true;
    this.choosedNote = id;
  }

  async returnToList(){
    this.addNewMode = false;
    this.showNoteMode = false;
    this.choosedNote = -1;
    this.decryptedNote = new Note();
    await this.loadNotes();
  }

  addNew(){
    this.addNewMode = true;
  }

  async addNote(form: NgForm){
    const credentials = {
      'title': form.value.title,
      'content': form.value.content,
      'public': this.publicChecked,
      'encrypted': this.encryptedChecked,
      'password': form.value.encryptionPassword
    }

    localStorage.setItem("public", String(this.publicChecked));
    localStorage.setItem("encrypted", String(this.encryptedChecked));

    try{
      let response = await this.http.post("http://localhost:8383/api/notes/add", credentials).toPromise();
      await this.returnToList();
    }
    catch(err){
      console.log(err);
    }
  }

  publicChange(event: any){
    this.publicChecked = event.target.checked;
  }

  encryptedChange(event: any){
    this.encryptedChecked = event.target.checked;
  }

  async decryptNote(form: NgForm){
    try{
      this.decryptedNote = await this.http.get<Note>("http://localhost:8383/api/notes/decrypt/" + this.choosedNote + "/" + form.value.password).toPromise();
    }
    catch(err){
      console.log(err);
    }

    console.log(this.decryptedNote.content);
  }
    
  showUserNotes(){
    this.publicNotesMode = false;
  }

  showPublic(){
    this.publicNotesMode = true;
  }

  returnToHomeScreen(){
    this.router.navigate(["/"]);
  }
}
