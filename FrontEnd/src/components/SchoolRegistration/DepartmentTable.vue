<template>
  <div id="DepartmentTable">
        <v-toolbar flat color="white">
          <v-toolbar-title>Department</v-toolbar-title>
          <v-divider class="mx-2" inset vertical></v-divider>
          <v-spacer></v-spacer>
          <v-dialog v-model="dialog" max-width="500px">
            <template v-slot:activator="{ on }">
              <v-btn color="primary" dark class="mb-2" v-on="on">New Item</v-btn>
            </template>
            <v-card>
              <v-card-title>
                <span class="headline">{{ formTitle }}</span>
              </v-card-title>

              <v-card-text>
                <v-container grid-list-md>
                  <v-layout wrap>
                    <v-flex xs12 sm6 md4>
                      <v-text-field v-model="editedItem.DepartmentName" label="Department Name"></v-text-field>
                    </v-flex>
                  </v-layout>
                </v-container>
              </v-card-text>

              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="blue darken-1" flat @click="close">Cancel</v-btn>
                <v-btn color="blue darken-1" flat @click="save">Save</v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
        </v-toolbar>
        <v-data-table :headers="headers" :items="departments" class="elevation-1">
          <template v-slot:items="props">
            <td>{{ props.item.DepartmentName }}</td>
            <td class="justify-center layout px-0">
              <v-icon small class="mr-2" @click="editItem(props.item)">edit</v-icon>
              <v-icon small @click="deleteItem(props.item)">delete</v-icon>
            </td>
          </template>
        </v-data-table>
      </div>
</template>
<script>
export default {
  data: () => ({
    dialog: false,
    headers: [
      {
        text: "Department Name",
        align: "left",
        sortable: false,
        value: "DepartmentName"
      },

    ],
    departments: [],
    editedIndex: -1,
    editedItem: {
      DepartmentName: "",
    },
    defaultItem: {
      DepartmentName: "",
    }
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "New Item" : "Edit Item";
    }
  },

  watch: {
    dialog(val) {
      val || this.close();
    },
    departments() {
      this.$eventBus.$emit("SendDepartmentTable", this.departments);
    }
  },

  created() {
    this.$eventBus.$on("DepartmentTableFromFile", DepartmentDTO => {
      this.departments = DepartmentDTO;
      /* eslint no-console: "off" */
    });
  },

  methods: {
    editItem(item) {
      this.editedIndex = this.departments.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialog = true;
    },

    deleteItem(item) {
      const index = this.departments.indexOf(item);
      confirm("Are you sure you want to delete this item?") &&
        this.departments.splice(index, 1);
    },

    close() {
      this.dialog = false;
      setTimeout(() => {
        this.editedItem = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      }, 300);
    },

    save() {
      if (this.editedIndex > -1) {
        Object.assign(this.departments[this.editedIndex], this.editedItem);
      } else {
        this.departments.push(this.editedItem);
      }
      this.close();
    }
  }
};
</script>
